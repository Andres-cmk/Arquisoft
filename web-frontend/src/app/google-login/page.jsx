import GoogleSignInRedirect from "@/components/GoogleSignInRedirect";

function normalizeCallbackUrl(value) {
  if (typeof value !== "string" || !value.startsWith("/")) {
    return "/dashboard";
  }

  return value;
}

export default async function GoogleLoginPage({ searchParams }) {
  const params = await searchParams;
  const callbackUrl = normalizeCallbackUrl(params?.callbackUrl);
  const selectAccount = params?.select_account === "1";

  return (
    <GoogleSignInRedirect
      callbackUrl={callbackUrl}
      selectAccount={selectAccount}
    />
  );
}
