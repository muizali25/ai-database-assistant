"use client";

import { useEffect, useState } from "react";
import ChatDialog from "@/components/ChatDialog";
import DataGrid from "@/components/DataGrid";
import { askQuestion, fetchTable, type Row } from "@/lib/api";
import { SAMPLE_QUESTIONS } from "@/lib/sampleQuestions";
import styles from "./page.module.css";

const TABLES = ["Products", "Categories", "Customers", "Sales"] as const;

type AiResult = {
  question: string;
  data: Row[];
  found: boolean;
};

export default function HomePage() {
  const [table, setTable] = useState<(typeof TABLES)[number]>("Products");
  const [rows, setRows] = useState<Row[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [aiResult, setAiResult] = useState<AiResult | null>(null);
  const [queryLoading, setQueryLoading] = useState(false);
  const [queryProgress, setQueryProgress] = useState(0);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      setLoading(true);
      setError("");
      try {
        const data = await fetchTable(table);
        if (!cancelled) {
          setRows(data);
          setAiResult(null);
        }
      } catch (err) {
        if (!cancelled) {
          setRows([]);
          setError(err instanceof Error ? err.message : "Failed to load data");
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    return () => {
      cancelled = true;
    };
  }, [table]);

  useEffect(() => {
    if (!queryLoading) {
      setQueryProgress(0);
      return;
    }

    setQueryProgress(8);
    const timer = window.setInterval(() => {
      setQueryProgress((prev) => (prev >= 90 ? prev : prev + Math.random() * 8 + 2));
    }, 350);

    return () => window.clearInterval(timer);
  }, [queryLoading]);

  async function runQuestion(question: string) {
    setQueryLoading(true);
    setAiResult(null);
    setError("");

    try {
      const result = await askQuestion(question);
      setQueryProgress(100);
      setAiResult({
        question,
        data: result.data,
        found: result.found,
      });
    } catch {
      setAiResult({
        question,
        data: [],
        found: false,
      });
    } finally {
      window.setTimeout(() => setQueryLoading(false), 250);
    }
  }

  const showingAi = !!aiResult && !queryLoading;
  const displayRows = showingAi && aiResult.found ? aiResult.data : rows;

  return (
    <main className={styles.main}>
      <header className={styles.header}>
        <div>
          <h1>Mini POS AI Query Assistant</h1>
          <p>Browse table data or ask a POS question — results appear here.</p>
        </div>

        <label className={styles.selectLabel}>
          Table
          <select
            value={table}
            onChange={(e) => setTable(e.target.value as (typeof TABLES)[number])}
            disabled={queryLoading}
          >
            {TABLES.map((name) => (
              <option key={name} value={name}>
                {name}
              </option>
            ))}
          </select>
        </label>
      </header>

      <div className={styles.layout}>
        <aside className={styles.menu}>
          <h2>Sample Questions</h2>
          <p className={styles.menuHint}>Click a question to run it on this page.</p>
          <ul className={styles.menuList}>
            {SAMPLE_QUESTIONS.map((q) => (
              <li key={q}>
                <button type="button" disabled={queryLoading} onClick={() => runQuestion(q)}>
                  {q}
                </button>
              </li>
            ))}
          </ul>
        </aside>

        <section className={styles.content}>
          {queryLoading && (
            <div className={styles.loaderPanel} aria-live="polite">
              <div className={styles.loaderTitle}>Running your question...</div>
              <div className={styles.progressTrack}>
                <div className={styles.progressBar} style={{ width: `${queryProgress}%` }} />
              </div>
              <p className={styles.loaderHint}>Validating and fetching results</p>
            </div>
          )}

          {showingAi && (
            <div className={styles.resultBanner}>
              <div>
                <strong>{aiResult.found ? "AI Query Result" : "Data not found"}</strong>
                <p>{aiResult.question}</p>
              </div>
              <button type="button" onClick={() => setAiResult(null)}>
                Back to {table}
              </button>
            </div>
          )}

          {!queryLoading && loading && !showingAi && (
            <p className={styles.status}>Loading {table}...</p>
          )}
          {!queryLoading && error && !showingAi && <p className={styles.error}>{error}</p>}

          {!queryLoading && showingAi && !aiResult.found && (
            <p className={styles.notFound}>Data not found</p>
          )}

          {!queryLoading && (!showingAi || aiResult.found) && !(!showingAi && error) && (
            <DataGrid
              rows={displayRows}
              emptyMessage={showingAi ? "Data not found" : "No records found."}
            />
          )}
        </section>
      </div>

      <ChatDialog disabled={queryLoading} onAsk={runQuestion} />
    </main>
  );
}
