# Performance Tests

These scripts implement the k6 option from `[SwArch_2026i] - Laboratory 6.pdf`.

The test target is the matchmaking feature through the API Gateway:

- API Gateway: Nginx reverse proxy.
- Microservice: `matchmaking`.
- Infrastructure deployed with database: PostgreSQL is part of the same `docker-compose.yml`.
- Extra broker used by the feature: RabbitMQ.
- Frontend deployed in the same compose: Next.js on port `3000`.

The k6 scenario creates a match and then consumes the next queued match. This exercises:

1. `POST /matchmaking/matches`
2. `GET /matchmaking/queue/next`

## Recommended Two-Machine Setup

Use two computers on the same LAN.

Machine A runs the system:

```powershell
docker compose up --build -d
```

Machine B runs k6. This follows the lab instruction that the load generator should run on a node independent from the system under test.

If you run k6 on the same machine as Docker, the test is still useful for development, but document it as a local test because CPU, RAM, and network resources are shared.

## Find Machine A IP

On Machine A:

```powershell
ipconfig
```

Use the IPv4 address of the active Wi-Fi or Ethernet adapter, for example:

```text
192.168.1.25
```

From Machine B, verify connectivity:

```powershell
Test-NetConnection 192.168.1.25 -Port 3000
Test-NetConnection 192.168.1.25 -Port 8001
curl.exe -k https://192.168.1.25:8001/health
curl.exe http://192.168.1.25:3000
```

If this fails:

- Make sure both computers are on the same network.
- Allow inbound Windows Firewall traffic on Machine A for TCP ports `3000`, `8000`, and `8001`.
- Check that Docker Desktop is running and `docker compose ps` shows healthy/running services.

On Machine A, you can create the firewall rules from an Administrator PowerShell:

```powershell
New-NetFirewallRule -DisplayName "Arquisoft web 3000" -Direction Inbound -Action Allow -Protocol TCP -LocalPort 3000
New-NetFirewallRule -DisplayName "Arquisoft support 8000" -Direction Inbound -Action Allow -Protocol TCP -LocalPort 8000
New-NetFirewallRule -DisplayName "Arquisoft matchmaking 8001" -Direction Inbound -Action Allow -Protocol TCP -LocalPort 8001
```

The local certificate is self-signed. k6 accepts it because the script sets `insecureSkipTLSVerify: true`. Some Windows `curl.exe` installations can still fail the HTTPS check with a Schannel error; in that case, rely on `Test-NetConnection` for port reachability and run the k6 smoke test below.

## Install k6 On Machine B

Windows options:

```powershell
winget install k6.k6
```

or:

```powershell
choco install k6
```

Verify:

```powershell
k6 version
```

Machine B needs this repository or at least the `performance/` folder.

## Generate Test Token

The matchmaking API requires `Authorization: Bearer <token>`. For performance testing, generate a token with the same `AUTH_TOKEN_SECRET` configured in `backend/.env` on Machine A.

Run this from the repository folder on Machine B, replacing the secret with the value from Machine A:

```powershell
$env:AUTH_TOKEN = python .\performance\generate_token.py --secret "<AUTH_TOKEN_SECRET>"
```

The script uses only Python standard library modules.

## Run Constant Load Tests

Replace the IP with Machine A's LAN IP.

1 user:

```powershell
k6 run `
  -e TARGET_BASE_URL=https://192.168.1.25:8001 `
  -e AUTH_TOKEN=$env:AUTH_TOKEN `
  -e PROFILE=constant `
  -e VUS=1 `
  -e DURATION=30s `
  .\performance\matchmaking_create_and_consume.js
```

50 users:

```powershell
k6 run `
  -e TARGET_BASE_URL=https://192.168.1.25:8001 `
  -e AUTH_TOKEN=$env:AUTH_TOKEN `
  -e PROFILE=constant `
  -e VUS=50 `
  -e DURATION=30s `
  .\performance\matchmaking_create_and_consume.js
```

Recommended sweep for the lab:

```powershell
foreach ($vus in 1, 50, 200, 500, 1000, 2000) {
  k6 run `
    -e TARGET_BASE_URL=https://192.168.1.25:8001 `
    -e AUTH_TOKEN=$env:AUTH_TOKEN `
    -e PROFILE=constant `
    -e VUS=$vus `
    -e DURATION=30s `
    --summary-export "performance-result-$vus.json" `
    .\performance\matchmaking_create_and_consume.js
}
```

## Run A Ramping Test

This is useful for finding the approximate knee of the curve in one run:

```powershell
k6 run `
  -e TARGET_BASE_URL=https://192.168.1.25:8001 `
  -e AUTH_TOKEN=$env:AUTH_TOKEN `
  -e PROFILE=stages `
  -e STAGES=30s:1,30s:50,30s:200,30s:500,30s:1000,30s:2000,30s:0 `
  .\performance\matchmaking_create_and_consume.js
```

For the final chart, prefer the constant load tests because each run gives a clean average for one concurrency level.

## Metrics To Document

For each VUS level, record:

| VUS | http_req_duration avg | http_req_duration p95 | http_req_failed | checks |
| --- | --- | --- | --- | --- |
| 1 | | | | |
| 50 | | | | |
| 200 | | | | |
| 500 | | | | |
| 1000 | | | | |
| 2000 | | | | |

Use:

- `http_req_duration avg` as the average response time.
- `http_req_duration p95` as a stricter latency indicator.
- `http_req_failed` to identify where the system starts failing.

The knee of the performance curve is below the first VUS level where latency grows sharply or `http_req_failed` becomes greater than `0%`.

## Reset Between Large Runs

The scenario consumes one RabbitMQ queue item after creating it, but large failed runs can leave queued messages. For clean repeated measurements, reset the deployment on Machine A:

```powershell
docker compose down
docker compose up --build -d
```

Then regenerate or reuse the token if `AUTH_TOKEN_SECRET` did not change.
