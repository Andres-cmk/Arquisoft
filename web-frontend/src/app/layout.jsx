import "./globals.css";
import Footer from "@/components/Footer";
import Navbar from "@/components/Navbar";
import { Cinzel, Newsreader } from "next/font/google";
import Script from "next/script";
import Providers from "@/components/Providers";

export const metadata = {
  title: "Crónicas de la Ciudad Blanca - Portal de la Academia",
  icons: {
    icon: "/icon.png",
  },
};

const cinzel = Cinzel({
  subsets: ["latin"],
  weight: ["400", "600", "700", "900"],
  variable: "--font-cinzel",
  display: "swap",
});

const newsreader = Newsreader({
  subsets: ["latin"],
  weight: ["400", "700"],
  style: ["normal", "italic"],
  variable: "--font-newsreader",
  display: "swap",
});

export default function RootLayout({ children }) {
  return (
    <html lang="es" className="dark" suppressHydrationWarning>
      <body
        suppressHydrationWarning
        className={`${newsreader.variable} ${cinzel.variable} font-body-md bg-background text-on-surface min-h-screen flex flex-col`}
      >
        <Script
          src="https://kit.fontawesome.com/8234d7916b.js"
          crossOrigin="anonymous"
          strategy="afterInteractive"
        />
        <Providers crossOrigin="anonymous">
          <Navbar />
          {children}
          <Footer />
        </Providers>
      </body>
    </html>
  );
}
