"use client";

import { signIn } from "next-auth/react";
import { useSearchParams } from "next/navigation";

export default function LoginForm() {
  const searchParams = useSearchParams();
  const callbackUrl = searchParams.get("callbackUrl") || "/dashboard";

  return (
    <button
      type="button"
      onClick={() => signIn("google", { callbackUrl })}
      className="w-full px-6 py-3 bg-primary text-on-primary border-2 border-outline-variant hover:brightness-110 transition-all font-headline-md"
    >
      <i className="fa-brands fa-google"></i> Continuar con Google
    </button>
  );
}
