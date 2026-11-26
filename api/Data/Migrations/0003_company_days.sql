-- Company days off, e.g. 24 December. Public holidays are calculated in code, not stored.
CREATE TABLE company_days (
    id   uuid PRIMARY KEY,
    date date NOT NULL UNIQUE,
    name text NOT NULL
);
