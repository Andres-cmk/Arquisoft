from pydantic import BaseModel, Field


class JoinQueueRequest(BaseModel):
    minPlayers: int = Field(default=2, ge=2, le=4)
    maxPlayers: int = Field(default=2, ge=2, le=4)
    gameMode: str = Field(default="standard", min_length=1, max_length=40)
    region: str = Field(default="default", min_length=1, max_length=40)


class ReadyRequest(BaseModel):
    ready: bool = True


class RelaySessionData(BaseModel):
    lobbyId: str | None = None
    lobbyCode: str | None = None
    relayJoinCode: str | None = None
    sessionName: str | None = None


class MatchPlayer(BaseModel):
    userId: int
    username: str
    role: str
    ready: bool


class MatchResponse(BaseModel):
    matchId: str
    status: str
    minPlayers: int
    maxPlayers: int
    gameMode: str
    region: str
    hostUserId: int
    players: list[MatchPlayer]
    relay: RelaySessionData | None = None
    createdAtUtc: str
    updatedAtUtc: str


class QueueJoinResponse(MatchResponse):
    role: str
