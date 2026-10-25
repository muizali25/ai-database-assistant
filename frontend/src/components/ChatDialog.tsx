"use client";

import { FormEvent, useState } from "react";
import { SAMPLE_QUESTIONS } from "@/lib/sampleQuestions";
import styles from "./ChatDialog.module.css";

type Props = {
  disabled?: boolean;
  onAsk: (question: string) => void;
};

export default function ChatDialog({ disabled = false, onAsk }: Props) {
  const [open, setOpen] = useState(false);
  const [question, setQuestion] = useState("");

  function close() {
    setOpen(false);
    setQuestion("");
  }

  function submit(raw: string) {
    const q = raw.trim();
    if (!q || disabled) return;
    close();
    onAsk(q);
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault();
    submit(question);
  }

  return (
    <>
      <button
        type="button"
        className={styles.fab}
        aria-label="Open AI assistant"
        onClick={() => setOpen(true)}
        disabled={disabled}
      >
        AI
      </button>

      {open && (
        <div className={styles.backdrop} onClick={close}>
          <div
            className={styles.dialog}
            onClick={(e) => e.stopPropagation()}
            role="dialog"
            aria-label="AI Query Assistant"
          >
            <header className={styles.header}>
              <h2>AI Query Assistant</h2>
              <button type="button" className={styles.close} onClick={close}>
                ×
              </button>
            </header>

            <div className={styles.messages}>
              <div className={styles.sampleMenu}>
                <p className={styles.hint}>Ask a question or pick a sample:</p>
                <ul className={styles.sampleList}>
                  {SAMPLE_QUESTIONS.map((q) => (
                    <li key={q}>
                      <button type="button" disabled={disabled} onClick={() => submit(q)}>
                        {q}
                      </button>
                    </li>
                  ))}
                </ul>
              </div>
            </div>

            <form className={styles.form} onSubmit={onSubmit}>
              <input
                value={question}
                onChange={(e) => setQuestion(e.target.value)}
                placeholder="Ask a question about the POS data..."
                disabled={disabled}
                autoFocus
              />
              <button type="submit" disabled={disabled || !question.trim()}>
                Ask
              </button>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
