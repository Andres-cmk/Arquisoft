import { redirect } from "next/navigation";
import { auth, signOut } from "@/auth";
import Message from "@/components/Message";

export default async function DashboardPage() {
  const session = await auth();

  if (!session?.user) {
    redirect("/login");
  }

  console.log("Sesión actual:", session);

  return (
    <main className="min-h-screen px-8 py-24">
      <div className="max-w-screen-container-max mx-auto bg-surface-container-low border-2 border-outline-variant p-8">
        <h1 className="font-headline-lg text-on-surface mb-2">Dashboard</h1>
        <p className="font-body-md text-on-surface-variant mb-6">
          Sesión iniciada como {session.user.email || session.user.name || "usuario"}.
        </p>

        <form
          action={async () => {
            "use server";
            await signOut({ redirectTo: "/" });
          }}
        >
          <button
            type="submit"
            className="px-6 py-3 border-2 border-outline-variant text-on-surface hover:bg-surface-container-high transition-all font-headline-md"
          >
            Cerrar sesión
          </button>
        </form>
        
        <div className="mt-8">
          <Message
            title={`¡Bienvenido, ${session.user.name || "erudito"}!`}
            description={`Sesión iniciada como ${session.user.email || "usuario"}.`}
          />
        </div>
      </div>
    </main>
  );
}
