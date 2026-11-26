-- A changed rule is a new row with a new start date. Old rows are never changed.
CREATE TABLE rules (
    id         uuid PRIMARY KEY,
    name       text NOT NULL,
    value      numeric NOT NULL,
    start_date date NOT NULL,
    UNIQUE (name, start_date)
);

-- Business Esbjerg values from docs/case.md. The new Holiday Act started 1 September 2020.
INSERT INTO rules (id, name, value, start_date) VALUES
    (gen_random_uuid(), 'DaysEarnedPerMonth',       2.08,  '2020-09-01'),
    (gen_random_uuid(), 'Week6HoursEarnedPerMonth', 3.083, '2020-09-01'),
    (gen_random_uuid(), 'SupplementPercent',        1,     '2020-09-01'),
    (gen_random_uuid(), 'RaisedSupplementPercent',  0.95,  '2020-09-01'),
    (gen_random_uuid(), 'RaisedSupplementPercent',  1.48,  '2024-05-01'),
    (gen_random_uuid(), 'HolidayPayPercent',        12.5,  '2020-09-01'),
    (gen_random_uuid(), 'Week6PayPercent',          2.5,   '2020-09-01'),
    (gen_random_uuid(), 'MainHolidayDays',          15,    '2020-09-01'),
    (gen_random_uuid(), 'NoticeMonthsMainHoliday',  3,     '2020-09-01'),
    (gen_random_uuid(), 'NoticeMonthsOtherHoliday', 1,     '2020-09-01');
