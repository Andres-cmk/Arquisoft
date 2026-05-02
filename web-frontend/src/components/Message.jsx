"use client";

import { useEffect, useState } from "react";

function Message({
  title = "¡Bienvenido!",
  description = "Has iniciado sesión correctamente.",
  durationMs = 3500,
}) {
  const [isRendered, setIsRendered] = useState(true);
  const [isVisible, setIsVisible] = useState(false);

  const close = () => {
    setIsVisible(false);
    setTimeout(() => setIsRendered(false), 300);
  };

  useEffect(() => {
    const enterTimer = setTimeout(() => setIsVisible(true), 10);
    const leaveTimer = setTimeout(() => setIsVisible(false), durationMs);
    const unmountTimer = setTimeout(() => setIsRendered(false), durationMs + 300);

    return () => {
      clearTimeout(enterTimer);
      clearTimeout(leaveTimer);
      clearTimeout(unmountTimer);
    };
  }, [durationMs]);

  if (!isRendered) return null;

  return (
    <div
      role="status"
      aria-live="polite"
      className={
        "fixed bottom-6 left-6 right-6 sm:left-auto sm:right-8 z-50 max-w-[28rem] transition-[transform,opacity] duration-300 " +
        (isVisible ? "opacity-100 translate-y-0" : "pointer-events-none opacity-0 translate-y-4")
      }
    >
      <div className="cursor-default flex items-start justify-between gap-4 w-full rounded-lg bg-[#232531] border border-white/10 px-4 py-3 shadow-2xl">
        <div className="flex gap-3 min-w-0">
          <div className="text-[#2b9875] bg-white/5 backdrop-blur-xl p-2 rounded-lg">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth="1.5"
              stroke="currentColor"
              className="w-5 h-5"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="m4.5 12.75 6 6 9-13.5"
              />
            </svg>
          </div>
          <div className="min-w-0">
            <p className="text-white font-headline-md truncate">{title}</p>
            <p className="text-gray-400 font-body-md truncate">{description}</p>
          </div>
        </div>
        <button
          type="button"
          aria-label="Cerrar mensaje"
          onClick={close}
          className="text-gray-400 hover:text-white hover:bg-white/5 p-1 rounded-md transition-colors"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            strokeWidth="1.5"
            stroke="currentColor"
            className="w-5 h-5"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M6 18 18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>
    </div>
  );
}

export default Message;
