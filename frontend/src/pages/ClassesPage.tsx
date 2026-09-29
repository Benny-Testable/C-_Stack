import { useEffect, useState } from 'react';
import { getJson } from '../api/client';
import { loadSession } from '../auth/session';

interface ClassRow {
  id: string;
  title: string;
  startsAtUtc: string;
  capacity: number;
}

export function ClassesPage() {
  const [rows, setRows] = useState<ClassRow[]>([]);
  const [notice, setNotice] = useState<string | null>(null);
  const session = loadSession();

  useEffect(() => {
    void getJson<ClassRow[]>('/api/classes', session?.token).then(setRows).catch(() => {
      setNotice('Class list is unavailable until the API is running.');
    });
  }, [session?.token]);

  return (
    <main>
      <h1>Class schedule</h1>
      {notice ? <p>{notice}</p> : null}
      <ul>
        {rows.map((row) => (
          <li key={row.id}>
            {row.title} — {row.startsAtUtc} — capacity {row.capacity}
          </li>
        ))}
      </ul>
      <p>Cancellations close two hours before the class starts.</p>
    </main>
  );
}
