import NextAuth from "next-auth";
import GoogleProvider from "next-auth/providers/google";
import { authConfig } from "../auth.config";

const PY_BACKEND_URL = process.env.PY_BACKEND_URL ?? "http://localhost:8000";

export const { handlers, auth, signIn, signOut } = NextAuth({
  ...authConfig,
  trustHost: true,
  providers: [
    GoogleProvider({
      clientId: process.env.AUTH_GOOGLE_ID!,
      clientSecret: process.env.AUTH_GOOGLE_SECRET!,
      allowDangerousEmailAccountLinking: true,
    }),
  ],

  callbacks: {
    ...authConfig.callbacks,

    async jwt({ token, account }) {

      if (account && account.provider === "google" && account.id_token) {
        try {
          const response = await fetch(`${PY_BACKEND_URL}/auth/google`, {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              id_token: account.id_token,
            }),
          });

          if (response.ok) {
            const data = await response.json();
            token.backendUserId = data.user_id;
            token.backendUsername = data.username;
            token.backendAccessToken = data.access_token;
          } else {
            console.error("Failed to authenticate with Google on the backend", {
              status: response.status,
              statusText: response.statusText,
            });
          }
        } catch (error) {
          console.error("Backend unreachable during Google sign-in", error);
        }
      }

      return token;
    },

    async session({ session, token }) {
      session.backendUserId = token.backendUserId as number | undefined;
      session.backendUsername = token.backendUsername as string | undefined;
      session.backendAccessToken = token.backendAccessToken as string | undefined;
      return session;
    }
  }
});
