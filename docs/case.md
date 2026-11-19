# Business Esbjerg: Absence Resource Planning

Business Esbjerg is a Business Development Council centered around the Esbjerg area. It has like most other businesses employees and has to abide by the Danish holiday legislation.

Previously Business Esbjerg had custom software for managing employee holiday resources, but the new legislation (in effect from 1st september 2020) made the software solution redundant and currently the resource management it is done using spreadsheets and email correspondences (which is very error prone and the user experience for the accounting department and employees is not optimal).

In essence, corporate wants a solution where employees can apply for holiday and the accounting department can approve and have overview of employee holiday resources. You as a developer must also propose how the solution should behave and what the interaction should be like.

Besides the holiday law enforced for all Danish companies, corporate also has the special "6th week of holiday" which is very typical for Danish companies. The main legislation points your software must abide by are:

## Two separate holiday "accounts"

A Danish employee has two holiday balances that follow different calendars. Most of the complexity comes from this. Think of each balance as a prepaid card that is topped up every month and spent on days off.

| | Weeks 1–5 (the Holiday Act) | Week 6 (the KL agreement) |
|---|---|---|
| Danish term | Samtidighedsferie (concurrent holiday) | 6. ferieuge (staggered, forskudt) |
| How much you earn | 2.08 days per month of employment = 25 days a year | 3.083 hours per month (full time, 37 h/week) = 37 hours = 5 days a year |
| Earning period | 1 September – 31 August | 1 January – 31 December (calendar year) |
| Period in which it must be used | 1 September – 31 December of the following year (16 months) | 1 May – 30 April after the calendar year it was earned in |
| Unit in the books | Days | Hours (a full-time day = 7.4 hours; part-time is proportional) |
| Required by law? | Yes, every employer | No, it's a collective agreement. Typical in Danish companies and the public sector |

## Main points from the law

**You earn holiday while you work and can use it almost immediately.** Since 2020, holiday for weeks 1–5 is earned and used "at the same time": a day earned in October can be taken in November. The 16-month usage period gives each earned day a long but fixed expiry date.

**Week 6 is a separate account with its own year.** Hours earned from January to December can only be used from 1 May the following year to 30 April the year after. Example: hours earned in 2026 can be used from 1 May 2027 to 30 April 2028. The week-6 balance can't be mixed with the weeks 1–5 balance.

**Holiday is paid, and the employee can't give it up.** A salaried employee keeps their normal salary while on holiday (ferie med løn). They also get a holiday supplement (ferietillæg): 1% under Ferieaftalen plus a raised supplement of 1.48% (since 1 May 2024) under 05.11. Hourly-paid staff and students in paid internships get a cash percentage instead (12.5% for weeks 1–5 and 2.5% for week 6). No employee can waive the right to holiday or holiday pay (05.11 § 3).

**Summer holiday comes first.** Everyone has the right to a main holiday (hovedferie) of 15 consecutive working days (3 weeks) between 1 May and 30 September. If possible, the other holiday days should be taken in blocks of at least 5 days. The employer decides when holiday is placed, but must consult the employee first. Notice must be given 3 months ahead for main holiday and 1 month ahead for other holiday. Week 6 is the exception: it is placed at the employee's request unless the work makes that impossible. It is taken in whole days, but subtracted from the balance in hours.

**Use it, transfer it, or have it paid out. Never just lose it.**

- The first 4 weeks must be taken as time off. They can't be paid out while you are employed. Only the 5th week can be transferred to the next period, and only by a written agreement made by 31 December after the period ends. A 5th week that is neither taken nor transferred is paid out. Holiday money for the first 4 weeks that was never used goes to a holiday fund. For municipalities this can be a local municipal holiday fund, and Esbjerg Kommune has its own Feriefond.
- Week 6 that isn't used by 30 April is paid out with the next salary (2.5% of the holiday-qualifying salary). It can also be transferred by a written agreement made before 30 September. Transferred hours must be used before newly earned ones. (05.11 §§ 11, 14, 16, 17)
- When an employee leaves, everything not yet taken is settled in cash with the final salary.

**Illness and other "holiday obstacles" (feriehindringer) protect the holiday.** If the employee is ill before the holiday starts, they don't have to start it and can take the days later. If they fall ill during weeks 1–5 holiday, they can get replacement days after the first 5 sick days, with documentation. If they fall ill during week 6, the employer may suspend the holiday in special cases and may ask for a doctor's note at the employer's cost (05.11 § 19). Parental leave, strikes and similar events are also holiday obstacles (05.11 § 21).

**Once approved, holiday is hard to take back.** The employer can only move an already planned week-6 holiday in force-majeure-like situations, must cover the employee's financial loss (for example a cancelled trip), and can never interrupt a holiday that has already started (05.11 § 10).

**Part-time and changes in working hours.** Holiday is earned in proportion to working hours. Week-6 hours are adjusted to the employee's working hours at the time the holiday is taken, while holiday pay follows the working hours at the time it was earned (05.11 §§ 8, 9 stk. 7, 12). Employees who start or leave mid-month earn holiday for the exact days worked (0.103 week-6 hours per calendar day, full time).

## What this means for the system

- Keep one ledger per account (weeks 1–5 and week 6), each with its own earning and usage periods, and monthly accrual (2.08 days / 3.083 hours, adjusted for part-time and mid-month start or end).
- A request must check the right account, the notice rules, the main-holiday rule (15 days between 1 May and 30 September), and public holidays and weekends, which never count as holiday days.
- Show employees and accounting expiry dates: 31 December for weeks 1–5 and 30 April for week 6. Flag balances that must be taken, transferred or paid out, and record written transfer agreements.
- Support the special cases: illness during or before a holiday, cancellation by the employer (with a record of the reason), leaving the company (final settlement), and hourly-paid staff (cash instead of paid time off).

## Corporate-specific requirements (Business Esbjerg)

The law and the agreement set the minimum. On top of that, corporate has the following practices and wishes for the solution.

### Requests are made in whole days

Employees request holiday in whole days only, not in single hours. A working day is not the same length every day:

| | Monday–Thursday | Friday | Full week |
|---|---|---|---|
| Working hours | 7.5 hours | 7 hours | 37 hours |

Balances are therefore kept in hours, and a holiday day costs exactly the hours of that weekday. A Friday off costs 7 hours, and a Tuesday costs 7.5 hours. One full week (Monday–Friday) always costs 37 hours, which matches what is earned in a year for week 6 (12 × 3.083 = 37 hours). Weeks 1–5 = 25 days = 5 × 37 = 185 hours a year for a full-time employee. The system has to turn "days" into "hours" using the employee's own work schedule. A part-time employee with a different schedule needs their own day lengths.

### Payout (udbetaling) is not something employees apply for

Holiday can be paid out instead of taken as time off, but only in specific situations. Employees never "request" a payout. The situation triggers it, and accounting has to handle it. The situations are:

- Resignation or dismissal (fratræden): everything that hasn't been taken is settled.
- The 5th week that hasn't been taken or transferred when the period ends (31 December).
- Week-6 hours that haven't been taken or transferred by 30 April.
- Holiday that couldn't be taken because of a holiday obstacle (illness, parental leave) at the end of the period.
- Retirement / leaving the labour market, and death (paid to the estate).

### Accounting must always know the remaining holiday in DKK

For each employee the system shows the remaining balance both in hours/days and as an amount in DKK (the holiday liability, feriepengeforpligtelse). This answers the question "what would we owe this person if they left today?". It must also be possible to add it up for the whole company. The amount is calculated for each account, using the salary in the earning period:

| | How the DKK value is calculated |
|---|---|
| Weeks 1–5 | 12.5% of the holiday-qualifying salary (ferieberettiget løn) earned in the period, multiplied by the share of earned days not yet taken |
| Week 6 | 2.5% of the holiday-qualifying salary in the calendar year, multiplied by the share of week-6 hours not yet taken (05.11 § 16) |
| Supplement | Any holiday supplement (ferietillæg) that hasn't been paid out yet |

> **Note:** For the first 4 weeks the money usually doesn't go straight to the employee when they leave. It is reported through Feriepengeinfo and released when the employee takes holiday later, unless they are leaving the labour market. The 5th and 6th weeks can be paid out directly. Either way accounting needs the exact amount. To calculate it, the system needs each employee's salary history and working hours month by month. The percentages and the method are parameters (see below), not hardcoded.

### Rules as configurable parameters, not hardcoded values

The rules change over time. For example, the raised holiday supplement went from 0.95% to 1.48% on 1 May 2024. The solution should therefore keep every number and rule as an admin-editable parameter with a start date (effective from). A new value applies from its start date and never rewrites history. Old balances and payouts are still calculated with the values that applied then.

Examples of parameters:

| Parameter | Business Esbjerg value |
|---|---|
| Weeks 1–5: days earned per month | 2.08 |
| Weeks 1–5: earning period / usage period | 1 Sep – 31 Aug / 1 Sep – 31 Dec the following year |
| Main holiday: length and window | 15 consecutive days, 1 May – 30 Sep |
| Notice: main holiday / other holiday | 3 months / 1 month |
| 6th week enabled | Yes |
| 6th week: hours earned per month (full time) | 3.083 |
| 6th week: earning period / usage period | 1 Jan – 31 Dec / 1 May – 30 Apr |
| Work schedule (hours per weekday) | Mon–Thu 7.5, Fri 7, Sat–Sun 0 |
| Request unit | Whole days |
| Holiday supplement (ferietillæg) | 1% + 1.48% |
| Payout percentages | 12.5% (weeks 1–5), 2.5% (week 6) |
| Transfer deadlines | 31 Dec (5th week), 30 Sep (week 6) |
| Calendar of public holidays and company days off | Danish public holidays + company-specific days (e.g. 24 and 31 December) |

### Rule presets

Typing every value by hand is error-prone, so the system ships with presets that an admin selects and then adjusts. Examples:

- **"Ferieloven (minimum)"**: 5 weeks, nothing else.
- **"Ferieloven + 6. ferieuge (KL 05.11 model)"**: what Business Esbjerg uses.
- **"Ferieloven + feriefridage"**: e.g. a collective agreement (overenskomst) such as Industriens Overenskomst with 5 extra holiday days (feriefridage).
- **"Hourly-paid staff"**: holiday pay as a percentage (feriegodtgørelse 12.5%) instead of salary during holiday.

A preset is a starting point. Changing a value afterwards makes it a custom rule set.

## Short glossary

| Danish | English |
|---|---|
| Ferieloven | The Holiday Act |
| Samtidighedsferie | Concurrent holiday (earn and use in the same period) |
| Optjeningsperiode / Afholdelsesperiode | Earning period / Period in which holiday is taken |
| Hovedferie / Restferie | Main (summer) holiday / Remaining holiday |
| 6. ferieuge | The sixth holiday week (agreement-based) |
| Ferie med løn / Feriegodtgørelse | Holiday with salary / Holiday pay as a percentage (cash) |
| Ferietillæg | Holiday supplement |
| Overførsel / Udbetaling | Transfer to the next period / Payout |
| Feriehindring | Holiday obstacle (illness, parental leave, …) |
| Fratræden | Leaving the job (resignation or dismissal) |
| Ferieberettiget løn | Holiday-qualifying salary (the salary the percentages are calculated from) |
| Feriepengeforpligtelse | The company's holiday liability (what remaining holiday is worth in DKK) |
| Feriepengeinfo | The national system where employers report holiday money owed when someone leaves |
| Overenskomst | Collective agreement between employers and a union |
| Feriefridage | Extra holiday days given by a collective agreement |
