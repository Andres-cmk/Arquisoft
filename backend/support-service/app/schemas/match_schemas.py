from datetime import datetime
from pydantic import BaseModel, Field

class ReceivedPayload(BaseModel):
    totalWood: int
    totalGold: int
    totalGatherActions: int
    elapsedSeconds: float
    startedAt: datetime
    finishedAt: datetime

class PayloadinDB(ReceivedPayload):
    id: int