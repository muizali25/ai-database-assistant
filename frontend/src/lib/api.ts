const API_BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5206";

export type Row = Record<string, unknown>;

export type AskResult = {
  found: boolean;
  message: string;
  data: Row[];
};

export async function fetchTable(tableName: string): Promise<Row[]> {
  const res = await fetch(`${API_BASE}/api/data/${tableName}`);
  if (!res.ok) {
    const body = await res.json().catch(() => ({}));
    throw new Error(body.error ?? `Failed to load ${tableName}`);
  }
  return res.json();
}

export async function askQuestion(question: string): Promise<AskResult> {
  const res = await fetch(`${API_BASE}/api/ai/ask`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ question }),
  });
  const body = await res.json().catch(() => ({}));
  if (!res.ok) {
    return { found: false, message: "Data not found", data: [] };
  }
  return {
    found: Boolean(body.found),
    message: (body.message as string) || (body.found ? "OK" : "Data not found"),
    data: (body.data ?? []) as Row[],
  };
}
