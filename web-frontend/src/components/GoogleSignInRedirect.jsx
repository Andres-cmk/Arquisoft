"use client";

import { useEffect } from "react";
import { signIn } from "next-auth/react";

export default function GoogleSignInRedirect({ callbackUrl, selectAccount }) {
  useEffect(() => {
    const authorizationParams = selectAccount
      ? { prompt: "select_account", max_age: "0" }
      : undefined;
    signIn("google", { callbackUrl }, authorizationParams);
  }, [callbackUrl, selectAccount]);

  return (
    <main className="min-h-screen flex items-center justify-center px-6 py-24">
      <section className="w-full max-w-md bg-surface-container-low border-2 border-outline-variant p-8">
        <h1 className="font-headline-lg text-on-surface mb-2">Redirigiendo</h1>
        <p className="font-body-md text-on-surface-variant">
          Abriendo Google para seleccionar cuenta.
        </p>
      </section>
    </main>
  );
}
