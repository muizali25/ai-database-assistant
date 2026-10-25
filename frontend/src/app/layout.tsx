import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Mini POS AI Query Assistant",
  description: "Browse POS tables and ask natural-language SQL questions",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
