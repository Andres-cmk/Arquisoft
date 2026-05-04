import { redirect } from "next/navigation";

import { auth } from "@/auth";

function isAllowedUnityRedirect(value) {
  if (!value) return false;

  try {
    const url = new URL(value);
    const isHttp = url.protocol === "http:" || url.protocol === "https:";
    const isLocalhost = url.hostname === "127.0.0.1" || url.hostname === "localhost";
    return isHttp && isLocalhost;
  } catch {
    return false;
  }
}

function buildUnityCallbackUrl(redirectUri, session, state) {
  const url = new URL(redirectUri);

  if (state) {
    url.searchParams.set("state", state);
  }

  url.searchParams.set("user_id", String(session.backendUserId));
  url.searchParams.set("username", session.backendUsername || session.user?.email || "user");
  url.searchParams.set("access_token", session.backendAccessToken);
  url.searchParams.set("token_type", "bearer");

  return url.toString();
}

function buildUnityLoginPath(redirectUri, state, options = {}) {
  const params = new URLSearchParams();
  params.set("redirect_uri", redirectUri);

  if (state) {
    params.set("state", state);
  }

  if (options.retry) {
    params.set("retry", "1");
  }

  if (options.selectAccount) {
    params.set("select_account", "1");
  }

  if (options.fresh) {
    params.set("fresh", "1");
  }

  return `/unity-login?${params.toString()}`;
}

function buildGoogleSignInPath(callbackUrl, selectAccount) {
  const params = new URLSearchParams();
  params.set("callbackUrl", callbackUrl);

  if (selectAccount) {
    params.set("select_account", "1");
  }

  return `/google-login?${params.toString()}`;
}

export default async function UnityLoginPage({ searchParams }) {
  const params = await searchParams;
  const redirectUri = params?.redirect_uri || "";
  const state = params?.state || "";
  const retry = params?.retry === "1";
  const selectAccount = params?.select_account === "1";
  const fresh = params?.fresh === "1";

  if (!isAllowedUnityRedirect(redirectUri)) {
    return (
      <main className="min-h-screen flex items-center justify-center px-6 py-24">
        <section className="w-full max-w-md bg-surface-container-low border-2 border-outline-variant p-8">
          <h1 className="font-headline-lg text-on-surface mb-2">Callback invalido</h1>
          <p className="font-body-md text-on-surface-variant">
            El retorno de Unity debe usar localhost o 127.0.0.1.
          </p>
        </section>
      </main>
    );
  }

  if (selectAccount && !fresh) {
    const callbackUrl = buildUnityLoginPath(redirectUri, state, {
      selectAccount: true,
      fresh: true,
    });
    redirect(buildGoogleSignInPath(callbackUrl, true));
  }

  const session = await auth();
  if (!session?.user) {
    const callbackUrl = buildUnityLoginPath(redirectUri, state, {
      selectAccount,
    });
    redirect(
      selectAccount
        ? buildGoogleSignInPath(callbackUrl, true)
        : `/login?callbackUrl=${encodeURIComponent(callbackUrl)}`
    );
  }

  if (!session.backendUserId || !session.backendAccessToken) {
    const callbackUrl = buildUnityLoginPath(redirectUri, state, {
      retry: true,
      selectAccount,
      fresh: true,
    });
    const retryUrl = buildGoogleSignInPath(callbackUrl, selectAccount);

    if (!retry) {
      redirect(retryUrl);
    }

    return (
      <main className="min-h-screen flex items-center justify-center px-6 py-24">
        <section className="w-full max-w-md bg-surface-container-low border-2 border-outline-variant p-8">
          <h1 className="font-headline-lg text-on-surface mb-2">Login incompleto</h1>
          <p className="font-body-md text-on-surface-variant">
            Google acepto la sesion, pero el backend no devolvio token del juego.
          </p>
          <a
            className="mt-6 inline-flex px-6 py-3 bg-primary text-on-primary border-2 border-outline-variant hover:brightness-110 transition-all font-headline-md"
            href={retryUrl}
          >
            Reintentar con Google
          </a>
        </section>
      </main>
    );
  }

  redirect(buildUnityCallbackUrl(redirectUri, session, state));
}
