import "next-auth";
import "next-auth/jwt";


declare module "next-auth" {
    interface Session {
        backendUserId?: number;
        backendUsername?: string;
        backendAccessToken?: string;
    }
}

declare module "next-auth/jwt" {
    interface JWT {
        backendUserId?: number;
        backendUsername?: string;
        backendAccessToken?: string;
    }
}
