CREATE TABLE IF NOT EXISTS Documents (
    id SERIAL PRIMARY KEY,
    source TEXT,
    category TEXT,
    shortName TEXT,
    fileName TEXT,
    content TEXT,
    fetchedAt TIMESTAMP
);
