import { Suspense } from "react";

import LoginForm from "@/components/LoginForm";

export default function LoginPage() {
  return (
    <main className="min-h-screen flex items-center justify-center px-6 py-24">
      <div className="w-full max-w-md bg-surface-container-low border-2 border-outline-variant p-8">
        <h1 className="font-headline-lg text-on-surface mb-2">Resgistro</h1>
        <p className="font-body-md text-on-surface-variant mb-6">
          Accede con tu cuenta de Google para continuar.
        </p>

        <Suspense
          fallback={
            <div className="w-full px-6 py-3 border-2 border-outline-variant text-on-surface-variant font-headline-md">
              Cargando...
            </div>
          }
        >
          <LoginForm />
        </Suspense>

        <p className="mt-4 text-xs text-on-surface-variant font-body-md">
          Serás redirigido automáticamente después de iniciar sesión.
        </p>
      </div>
    </main>
  );
}
