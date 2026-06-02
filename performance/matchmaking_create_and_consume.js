import http from "k6/http";
import { check, sleep } from "k6";
import exec from "k6/execution";

const targetBaseUrl = (__ENV.TARGET_BASE_URL || "https://localhost:8001").replace(/\/$/, "");
const authToken = __ENV.AUTH_TOKEN || "";
const profile = __ENV.PROFILE || "constant";
const vus = Number(__ENV.VUS || "1");
const duration = __ENV.DURATION || "30s";
const sleepSeconds = Number(__ENV.SLEEP_SECONDS || "1");

function parseStages(value) {
  return value.split(",").map((entry) => {
    const [stageDuration, target] = entry.split(":");
    return {
      duration: stageDuration.trim(),
      target: Number(target.trim()),
    };
  });
}

function scenarioOptions() {
  if (profile === "stages") {
    return {
      matchmaking: {
        executor: "ramping-vus",
        stages: parseStages(__ENV.STAGES || "30s:1,30s:50,30s:200,30s:500,30s:0"),
      },
    };
  }

  return {
    matchmaking: {
      executor: "constant-vus",
      vus,
      duration,
    },
  };
}

export const options = {
  insecureSkipTLSVerify: (__ENV.INSECURE_SKIP_TLS_VERIFY || "true").toLowerCase() !== "false",
  scenarios: scenarioOptions(),
  thresholds: {
    http_req_failed: ["rate<0.05"],
    http_req_duration: ["p(95)<2000"],
  },
};

export default function () {
  if (!authToken) {
    throw new Error("Missing AUTH_TOKEN. Generate it with performance/generate_token.py.");
  }

  const headers = {
    Authorization: `Bearer ${authToken}`,
    "Content-Type": "application/json",
  };

  const uniqueId = `${exec.scenario.name}-${exec.vu.idInTest}-${exec.scenario.iterationInTest}-${Date.now()}`;
  const createPayload = JSON.stringify({
    gameMode: "performance",
    region: "lan",
    maxPlayers: 2,
    relayJoinCode: `K6-${uniqueId}`,
  });

  const createRes = http.post(`${targetBaseUrl}/matchmaking/matches`, createPayload, {
    headers,
    tags: { operation: "create_match" },
  });

  check(createRes, {
    "create status is 201": (response) => response.status === 201,
    "create returns matchId": (response) => {
      try {
        return Boolean(response.json("matchId"));
      } catch {
        return false;
      }
    },
  });

  const nextRes = http.get(`${targetBaseUrl}/matchmaking/queue/next`, {
    headers,
    tags: { operation: "consume_queue" },
  });

  check(nextRes, {
    "queue status is 200": (response) => response.status === 200,
  });

  sleep(sleepSeconds);
}
