import "next-auth";
import "next-auth/jwt";


declare module "next-auth" {
    interface Session {
        backendUserId?: number;
    }
}

declare module "next-auth/jwt" {
    interface JWT {
        backendUserId?: number;
    }
}
