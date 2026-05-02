"use client";

function Footer() {
    return (
        <footer className="bg-slate-900 border-t-8 border-[#124559] shadow-[inner_0_10px_20px_rgba(0,0,0,0.8)]">
            <div className="flex flex-col md:flex-row justify-between items-center w-full px-12 py-10 gap-6 max-w-screen-2xl mx-auto">
                <div className="flex flex-col gap-2">
                    <span className="text-lg font-black text-slate-400 font-headline-md uppercase tracking-widest">
                        Crónicas de la Ciudad Blanca
                    </span>
                    <p className="font-body-md text-xs italic tracking-tighter text-slate-500">
                        © 1267 Real Universidad de la Ciudad Blanca. Custodiado por la Orden
                        de Minerva.
                    </p>
                </div>

                <div className="flex flex-wrap justify-center gap-8">
                    <a
                        className="text-slate-500 font-body-md text-xs italic tracking-tighter hover:text-[#7DC2FD] hover:-translate-y-px transition-transform"
                        href="#"
                    >
                        Archivos Universitarios
                    </a>
                    <a
                        className="text-slate-500 font-body-md text-xs italic tracking-tighter hover:text-[#7DC2FD] hover:-translate-y-px transition-transform"
                        href="#"
                    >
                        Créditos Reales
                    </a>
                    <a
                        className="text-slate-500 font-body-md text-xs italic tracking-tighter hover:text-[#4F772D] hover:-translate-y-px transition-transform"
                        href="#"
                    >
                        Gremio de Alquimia
                    </a>
                    <a
                        className="text-slate-500 font-body-md text-xs italic tracking-tighter hover:text-[#4F772D] hover:-translate-y-px transition-transform"
                        href="#"
                    >
                        Código de Honor
                    </a>
                </div>
            </div>
        </footer>
    );
}

export default Footer;