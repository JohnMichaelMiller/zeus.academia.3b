---
ai_generated: true
model: "anthropic/claude-sonnet-4.6@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-25-create-prd-prompt"
prompt: |
  create a new prompt file containing the steps to create a structured PRD with problem, goals, non-goals, and success metrics
started: "2026-02-25T00:00:00Z"
ended: "2026-02-25T00:10:00Z"
task_durations:
  - task: "design PRD structure"
    duration: "00:04:00"
  - task: "draft prompt content"
    duration: "00:04:00"
  - task: "add provenance and finalize"
    duration: "00:02:00"
total_duration: "00:10:00"
ai_log: "ai-logs/2026/02/25/2026-02-25-create-prd-prompt/conversation.md"
source: "johnmillerATcodemag-com"
description: "Prompt for generating a structured Product Requirements Document (PRD)"
context: "Academic Management System (zeus.academia) — product planning and feature definition"
expected_output: "A complete PRD Markdown document with problem statement, goals, non-goals, and success metrics"
tools: ["read", "search", "create", "edit"]
mode: agent
name: create-prd
author: John Miller
tags: [prd, product, requirements, planning, goals, metrics, product-manager]
arguments:
  - name: feature_name
    description: Name of the feature or initiative being defined
  - name: author
    description: Name or username of the PRD author
  - name: target_users
    description: Primary user segment(s) this feature targets (e.g., students, instructors, admins)
  - name: context_files
    description: Optional comma-separated list of context files to read before drafting (e.g., existing specs, user stories)
examples:
  - input: "create-prd feature_name=Enrollment Notifications author=jmiller target_users=students"
    output: "PRD Markdown document for Enrollment Notifications with all required sections"
---

# Prompt: Create a Structured PRD

Generate a Product Requirements Document (PRD) for **{{feature_name}}** authored by **{{author}}**.

## Objective

Produce a complete, structured PRD Markdown file at `docs/prd/{{feature_name | kebab-case}}.prd.md` that captures the full scope of the feature and provides sufficient clarity for design, engineering, and stakeholder alignment.

## Pre-Work

1. If `{{context_files}}` is provided, read each file and extract relevant background — existing workflows, constraints, prior decisions.
2. Identify the primary user segment(s): **{{target_users}}**.
3. Ask one clarifying question if the problem statement is ambiguous before proceeding.

## PRD Structure

Produce a Markdown document with the following sections, in order:

---

### 1. Metadata Header

```markdown
---
feature: <feature name>
author: <author>
date: <YYYY-MM-DD>
status: Draft | In Review | Approved
version: 1.0
---
```

---

### 2. Overview

One paragraph (3–5 sentences) summarising what this PRD covers and why it matters. State the feature name, the user segment affected, and the desired outcome.

---

### 3. Problem Statement

Answer these questions explicitly:

- **What is the current experience?** Describe the pain point or gap as users experience it today.
- **Who is affected?** Name the user segment(s) and approximate scope (e.g., "all enrolled students").
- **What is the impact of not solving this?** Quantify or qualify the cost of inaction (user frustration, lost efficiency, compliance risk, etc.).

Format as structured prose or a numbered list. No bullet soup.

---

### 4. Goals

List 3–6 specific, outcome-oriented goals. Each goal must be:

- Phrased as a desired outcome, not a solution
- Measurable or verifiable
- Scoped to this initiative

**Format:**

```markdown
| #   | Goal                | Measurement Proxy           |
| --- | ------------------- | --------------------------- |
| 1   | <outcome statement> | <how we know it's achieved> |
```

---

### 5. Non-Goals

List explicit boundaries — what this feature will **not** do. Non-goals prevent scope creep and set stakeholder expectations.

Include at least 3 non-goals. Format as a bulleted list with a one-sentence rationale for each.

**Format:**

```markdown
- **<Non-goal>**: <Why it is out of scope for this initiative>
```

---

### 6. Success Metrics

Define 3–5 quantifiable success metrics. Each metric must include:

- **Metric name**
- **Definition** — what is being measured and how
- **Baseline** — current value (use "TBD" if unknown)
- **Target** — desired value at 30/60/90 days or at launch
- **Owner** — team or role responsible for tracking

**Format:**

```markdown
| Metric | Definition   | Baseline       | Target  | Owner  |
| ------ | ------------ | -------------- | ------- | ------ |
| <name> | <definition> | <value or TBD> | <value> | <team> |
```

---

### 7. User Stories (Optional but Recommended)

List 2–5 user stories in standard format:

```
As a <user type>, I want to <action> so that <outcome>.
```

Include acceptance criteria for each story as a checklist.

---

### 8. Open Questions

List any unresolved questions, assumptions to validate, or dependencies to confirm. Use a numbered list with an owner and due date for each.

---

### 9. Appendix (Optional)

Link to or inline supporting artifacts: wireframes, research findings, prior PRDs, related tickets.

---

## Output Requirements

- **File path**: `docs/prd/{{feature_name | kebab-case}}.prd.md`
- **Format**: Valid Markdown with YAML front matter
- **Length**: 400–800 words of prose; tables as needed
- **Tone**: Structured, concise, outcome-oriented; avoid implementation prescriptions in problem/goals sections

## Validation Checklist

- [ ] Problem statement answers all three questions (current experience, who, impact)
- [ ] All goals are outcome-oriented and measurable
- [ ] At least 3 non-goals with rationale
- [ ] All success metrics have a baseline, target, and owner
- [ ] No solutions are prescribed in the Problem or Goals sections
- [ ] No undefined acronyms or jargon
- [ ] YAML front matter is valid and complete
