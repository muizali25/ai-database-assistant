"use client";

import type { Row } from "@/lib/api";
import styles from "./DataGrid.module.css";

type Props = {
  rows: Row[];
  emptyMessage?: string;
};

export default function DataGrid({ rows, emptyMessage = "No records found." }: Props) {
  if (!rows.length) {
    return <p className={styles.empty}>{emptyMessage}</p>;
  }

  const columns = Object.keys(rows[0]);

  return (
    <div className={styles.wrap}>
      <table className={styles.table}>
        <thead>
          <tr>
            {columns.map((col) => (
              <th key={col}>{col}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row, i) => (
            <tr key={i}>
              {columns.map((col) => (
                <td key={col}>{formatCell(row[col])}</td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function formatCell(value: unknown): string {
  if (value === null || value === undefined) return "";
  if (typeof value === "object") return JSON.stringify(value);
  return String(value);
}
