"use client";
import Image from "next/image";

function BodyHome() {
  return (
    <>
      <main>
        {/* Hero Section */}
        <section className="relative h-screen flex items-center justify-center overflow-hidden">
          <div
            className="absolute inset-0 bg-cover bg-center bg-no-repeat grayscale-[0.1] brightness-[0.5]"
            style={{ backgroundImage: "url('/back.png')" }}
          />
          <div className="absolute inset-0 bg-linear-to-t from-background via-background/40 to-background/60" />

          <div className="relative z-10 text-center px-7 max-w-4xl pt-24 p-20">
            <div className="inline-block mb-6 border-y-2 border-[#7DC2FD]/30 py-2 px-8">
              <span className="font-headline-md text-[#7DC2FD] tracking-[0.3em] text-sm uppercase">
                Real Universidad de la Ciudad Blanca
              </span>
            </div>

            <h1 className="font-headline-xl text-white drop-shadow-[0_5px_15px_rgba(0,0,0,1)] text-5xl md:text-7xl mb-8 leading-none">
              <span className="block text-transparent bg-clip-text bg-linear-to-b from-[#ffdcc4] via-[#eabe9f] to-[#5f4029] drop-shadow-[2px_4px_0px_rgba(0,0,0,0.8)] filter contrast-125 uppercase">
                Crónicas de la
              </span>{" "}
              <span className="block text-transparent bg-clip-text bg-linear-to-b from-[#ffdcc4] via-[#eabe9f] to-[#5f4029] drop-shadow-[2px_4px_0px_rgba(0,0,0,0.8)] filter contrast-125 uppercase">
                Ciudad Blanca
              </span>
            </h1>

            <p className="font-body-lg text-on-surface-variant mb-12 max-w-2xl mx-auto italic drop-shadow-md text-white">
              Donde la piedra habla de antiguos misterios y el conocimiento es
              la única moneda que compra la eternidad.
            </p>

            <div className="flex flex-col sm:flex-row gap-6 justify-center items-center">
              <button className="px-12 py-5 bg-[#4F772D] text-white border-4 border-[#124559] font-headline-md text-xl hover:brightness-125 transition-all flex items-center  ">
                Iniciar Campaña
              </button>
              <button className="px-12 py-5 border-4 border-[#7DC2FD]/50 text-[#7DC2FD] font-headline-md text-xl hover:bg-[#124559]/80 hover:text-white transition-all">
                Explorar Lore
              </button>
            </div>
          </div>

          <div className="absolute bottom-10 left-1/2 -translate-x-1/2 animate-bounce"></div>
        </section>

        {/* Story/Introduction */}
        <section className="py-24 px-8 relative bg-surface-container-low overflow-hidden">
          <div className="max-w-screen-container-max mx-auto relative">
            <div className="absolute -top-12 -left-12 w-48 h-48 border-t-8 border-l-8 border-[#124559] opacity-50" />
            <div className="absolute -bottom-12 -right-12 w-48 h-48 border-b-8 border-r-8 border-[#4F772D] opacity-50" />

            <div className="bg-[#FEFAE0] parchment-texture border-12 border-[#124559] shadow-[0_20px_50px_rgba(0,0,0,0.6)] p-12 md:p-24 relative">
              <div className="absolute top-0 left-0 w-full h-4 bg-linear-to-b from-black/10 to-transparent" />

              <div className="grid md:grid-cols-2 gap-16 items-center">
                <div className="space-y-8">
                  <h2 className="font-headline-lg text-[#124559] border-b-2 border-[#124559]/20 pb-4">
                    La Leyenda de la Ciudad Blanca
                  </h2>

                  <div className="font-body-lg text-[#124559]/90 space-y-6 leading-relaxed">
                    <p>
                      Desde el siglo XIII, la Real Universidad ha permanecido
                      como el último bastión contra la oscuridad de la
                      ignorancia. Sus muros de piedra blanca, tallados por
                      artesanos olvidados, guardan los secretos de la alquimia,
                      el derecho divino y la geometría sagrada.
                    </p>
                    <p className="italic font-bold text-[#4F772D]">
                      “En el silencio de la biblioteca, el eco de los que fueron
                      grita el camino de los que vendrán.”
                    </p>
                    <p>
                      Hoy, la Ciudad Blanca no es solo un lugar de estudio, sino
                      el campo de batalla donde las facciones luchan por el
                      control de la Verdad Única. Tu legado comienza aquí, en
                      los pasillos donde el tiempo se detiene.
                    </p>
                  </div>
                </div>

                <div className="relative">
                  <Image
                    className="w-full h-auto rounded-sm border-4 border-[#124559] shadow-xl grayscale-[0.2] sepia-[0.2]"
                    alt="Plaza de la Universidad"
                    src="/back.png"
                    width={600}
                    height={400}
                  />
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* Feature Cards: Faculties */}
        <section className="py-24 bg-surface px-8">
          <div className="max-w-screen-container-max mx-auto">
            <div className="text-center mb-16">
              <h2 className="font-headline-lg text-[#7DC2FD] mb-4 tracking-widest uppercase">
                Las Tres Facultades
              </h2>
              <p className="font-body-md text-on-surface-variant max-w-xl mx-auto">
                Elige tu camino dentro de la Academia. Cada símbolo representa
                una fuerza que moldea el destino de la Ciudad.
              </p>
            </div>

            <div className="grid md:grid-cols-3 gap-12">
              <div className="flex flex-col items-center group">
                <div className="w-full bg-[#FEFAE0] parchment-texture border-4 border-[#124559] p-8 transition-transform group-hover:-translate-y-4 shadow-lg text-center relative overflow-hidden">
                  <div className="mb-6 relative h-32 flex items-center justify-center">
                    <span className="material-symbols-outlined text-7xl text-[#124559] group-hover:text-[#7DC2FD] transition-colors">
                      Imagen
                    </span>
                  </div>
                  <h3 className="font-headline-md text-[#124559] mb-4">
                    Imagen
                  </h3>
                  <p className="font-body-md text-[#124559]/80 mb-6">
                    Custodios de la ley y el orden imperial. Buscan el
                    equilibrio a través del juicio implacable y la diplomacia.
                  </p>
                  <button className="font-headline-md text-xs tracking-widest uppercase text-[#124559] border-b border-[#124559] pb-1 hover:text-[#7DC2FD] hover:border-[#7DC2FD] transition-all">
                    Saber más
                  </button>
                </div>
              </div>

              <div className="flex flex-col items-center group">
                <div className="w-full bg-[#FEFAE0] parchment-texture border-4 border-[#124559] p-8 transition-transform group-hover:-translate-y-4 shadow-lg text-center relative overflow-hidden">
                  <div className="mb-6 relative h-32 flex items-center justify-center">
                    <span className="material-symbols-outlined text-7xl text-[#124559] group-hover:text-[#4F772D] transition-colors">
                      Imagen
                    </span>
                  </div>
                  <h3 className="font-headline-md text-[#124559] mb-4">
                    Orden de la Serpiente
                  </h3>
                  <p className="font-body-md text-[#124559]/80 mb-6">
                    Alquimistas y herbolarios que operan en las sombras de la
                    cripta. El conocimiento prohibido es su única meta.
                  </p>
                  <button className="font-headline-md text-xs tracking-widest uppercase text-[#124559] border-b border-[#124559] pb-1 hover:text-[#4F772D] hover:border-[#4F772D] transition-all">
                    Saber más
                  </button>
                </div>
              </div>

              <div className="flex flex-col items-center group">
                <div className="w-full bg-[#FEFAE0] parchment-texture border-4 border-[#124559] p-8 transition-transform group-hover:-translate-y-4 shadow-lg text-center relative overflow-hidden">
                  <div className="mb-6 relative h-32 flex items-center justify-center">
                    <span className="material-symbols-outlined text-7xl text-[#124559] group-hover:text-[#5A3C25] transition-colors">
                      Imagen
                    </span>
                  </div>
                  <h3 className="font-headline-md text-[#124559] mb-4">
                    Círculo del Ratio
                  </h3>
                  <p className="font-body-md text-[#124559]/80 mb-6">
                    Matemáticos y arquitectos que ven el mundo como una gran
                    ecuación. La precisión es su arma y su escudo.
                  </p>
                  <button className="font-headline-md text-xs tracking-widest uppercase text-[#124559] border-b border-[#124559] pb-1 hover:text-[#5A3C25] hover:border-[#5A3C25] transition-all">
                    Saber más
                  </button>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* Final CTA Section */}
        <section className="py-32 px-8 bg-surface-container-high relative overflow-hidden border-y-8 border-[#124559]">
          <div
            className="absolute inset-0 opacity-10 grayscale pointer-events-none"
            style={{
              backgroundImage: "url('/back.png')",
              backgroundSize: "cover",
            }}
          />
          <div className="max-w-4xl mx-auto relative z-10">
            <div className="bg-surface-container-low border-2 border-[#124559] p-12 stone-bevel rounded-none">
              <div className="text-center mb-12">
                <h2 className="font-headline-lg text-[#7DC2FD] mb-2">
                  Ingresar a la Academia
                </h2>
                <p className="font-body-md text-[#7DC2FD]/70">
                  Tu nombre será grabado en los anales de la Ciudad Blanca.
                </p>
              </div>

              <form className="grid md:grid-cols-2 gap-8">
                <div className="space-y-2">
                  <label className="font-headline-md text-xs text-[#4F772D] uppercase tracking-widest">
                    Nombre del Erudito
                  </label>
                  <input
                    className="w-full bg-surface-container-highest border-2 border-[#124559] text-on-surface p-4 focus:ring-2 focus:ring-[#7DC2FD] focus:outline-none transition-all"
                    placeholder="Ej: Fray Bartolomé"
                    type="text"
                  />
                </div>

                <div className="space-y-2">
                  <label className="font-headline-md text-xs text-[#4F772D] uppercase tracking-widest">
                    Correo de la Orden
                  </label>
                  <input
                    className="w-full bg-surface-container-highest border-2 border-[#124559] text-on-surface p-4 focus:ring-2 focus:ring-[#7DC2FD] focus:outline-none transition-all"
                    placeholder="correo@universidad.edu"
                    type="email"
                  />
                </div>

                <div className="md:col-span-2 space-y-2">
                  <label className="font-headline-md text-xs text-[#4F772D] uppercase tracking-widest">
                    Facultad de Interés
                  </label>
                  <select className="w-full bg-surface-container-highest border-2 border-[#124559] text-on-surface p-4 focus:ring-2 focus:ring-[#7DC2FD] focus:outline-none transition-all appearance-none">
                    <option>Gremio de la Balanza</option>
                    <option>Orden de la Serpiente</option>
                    <option>Círculo del Ratio</option>
                  </select>
                </div>

                <div className="md:col-span-2 pt-6">
                  <button
                    className="w-full py-5 bg-[#4F772D] text-white border-4 border-[#124559] font-headline-md text-xl wax-seal hover:bg-[#124559] transition-all shadow-2xl flex items-center justify-center gap-4"
                    type="button"
                  >
                    <i className="fa-solid fa-book-open"></i> Sellar Registro
                  </button>
                </div>
              </form>
            </div>
          </div>
        </section>
      </main>
    </>
  );
}

export default BodyHome;
