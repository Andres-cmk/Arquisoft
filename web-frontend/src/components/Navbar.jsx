"use client";
import Link from "next/link";
import Image from "next/image";
import { usePathname } from "next/navigation";
import { useSession } from "next-auth/react";

function Navbar() {
  const pathname = usePathname();
  const isOnDashboard = pathname?.startsWith("/dashboard");
  const { data: session } = useSession();
  const user = session?.user;

  return (
    <>
      <nav className="fixed top-0 left-0 w-full z-50 bg-slate-950/95 border-b-4 border-[#124559] shadow-2xl">
        <div className="flex justify-between items-center w-full px-8 py-2 max-w-screen-2xl mx-auto">
          <div className="flex items-center gap-4">
            <Link className="flex items-center gap-3" href="/">
              <Image
                src="/logo.png"
                alt="Crónicas de la Ciudad Blanca"
                className="h-10 w-10 object-contain filter brightness-110"
                width={20}
                height={20}
              />
            </Link>
          </div>
          <div className="hidden md:flex items-center gap-8">
            <Link
              className="text-[#7DC2FD] border-b-2 border-[#7DC2FD] pb-1 font-body-md tracking-widest uppercase text-sm hover:text-sky-200 hover:drop-shadow-[0_0_10px_rgba(125,194,253,0.5)] transition-all duration-300"
              href="#"
            >
              Lore
            </Link>
            <Link
              className="text-amber-50/80 font-body-md tracking-widest uppercase text-sm hover:text-[#7DC2FD] hover:drop-shadow-[0_0_10px_rgba(125,194,253,0.5)] transition-all duration-300"
              href="#"
            >
              Campañas
            </Link>
            <Link
              className="text-amber-50/80 font-body-md tracking-widest uppercase text-sm hover:text-[#7DC2FD] hover:drop-shadow-[0_0_10px_rgba(125,194,253,0.5)] transition-all duration-300"
              href="#"
            >
              Comunidad
            </Link>
          </div>
          <div className="flex items-center gap-8">
            {isOnDashboard && user ? (
              <Link
                href="/dashboard"
                className="flex items-center gap-3 text-amber-50/90 hover:text-[#7DC2FD] transition-all duration-300"
              >
                {user.image ? (
                  <Image
                    src={user.image}
                    alt={user.name || user.email || "Perfil"}
                    className="h-10 w-10 rounded-full border-2 border-[#7DC2FD]/50 object-cover"
                    width={40}
                    height={40}
                    referrerPolicy="no-referrer"
                  />
                ) : (
                  <div className="h-10 w-10 rounded-full border-2 border-[#7DC2FD]/50 bg-[#124559]" />
                )}
                <span className="font-body-md tracking-widest uppercase text-sm">
                  {user.name || user.email || "Usuario"}
                </span>
              </Link>
            ) : (
              <>
                <button className="bg-[#124559] text-white px-3 py-2 wax-seal hover:bg-[#4F772D] tracking-widest uppercase hover:scale-105 active:brightness-75 transition-all font-headline-md text-sm border border-[#7DC2FD]/50">
                  Iniciar Campaña
                </button>
                <Link
                  className="text-amber-50/80 font-body-md tracking-widest uppercase text-sm hover:text-[#7DC2FD] hover:drop-shadow-[0_0_10px_rgba(125,194,253,0.5)] transition-all duration-300"
                  href="/login"
                >
                  Registrarse
                </Link>
              </>
            )}
          </div>
        </div>
      </nav>
    </>
  );
}

export default Navbar;
