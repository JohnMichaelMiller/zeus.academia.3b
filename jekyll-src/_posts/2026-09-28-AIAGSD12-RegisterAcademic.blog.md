---
ai_generated: true
model: "github/copilot@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "edaf3bc8-e80f-4b74-b582-2aa3785f6a40"
prompt: |
  create a blog post describing the Part-Twelve code changes
started: "2026-09-29T00:51:52.5436296Z"
ended: "2026-09-29T01:00:52.6369587Z"
task_durations:
  - task: "article research and drafting"
    duration: "00:09:00"
total_duration: "00:09:00"
ai_log: "ai-logs/2026/09/28/edaf3bc8-e80f-4b74-b582-2aa3785f6a40/conversation.md"
source: "user request: create a blog post describing the Part-Twelve code changes"
layout: post
title: "AI-Assisted Greenfield Software Development, Part 12: Registering Academics Across Vertical Slices"
date: 2026-09-28
categories: [ai-assisted-development, software-engineering, architecture]
tags: [vertical-slices, mediatR, ef-core, sql-server, cqrs]
excerpt: "Part 12 implements academic registration by composing owner-managed reference data with one atomic write and explicit migration boundaries."
description: "I implement the RegisterAcademic vertical slice, resolve degree and university codes through their owning features, and preserve schema ownership while testing the end-to-end SQL Server workflow."
image: /assets/images/2026-09-28/AIAGSD12_RegisterAcademic.svg
---

Part 11 closed the reference-data implementation sequence and pointed to `RegisterAcademic` as the next domain slice. In Part 12, I implement that registration route and work through the boundary question it raises: how can one request validate several feature-owned values and persist a consistent result without taking ownership of every table it touches?

<!--more-->

<figure>
  <img src="/assets/images/2026-09-28/AIAGSD12_RegisterAcademic.svg" alt="RegisterAcademic validates a request, resolves reference data through owning features, and writes academic and extension state while schema migrations remain with table owners." />
  <figcaption>RegisterAcademic coordinates the workflow; each feature keeps ownership of its schema.</figcaption>
</figure>

## Why Academic Registration Needs an Orchestrator

Registration crosses several established boundaries. A rank comes from the rank catalog, a degree and university must resolve against their reference-data features, and an extension must still be available when the academic is created. If the registration feature copied those catalogs or migrated those tables, the same facts would acquire competing owners.

I kept the operation in a focused `Academics/RegisterAcademic` vertical slice. Its handler coordinates validation and writes, while MediatR queries let reference-data features answer questions about the records they own. That keeps the workflow cohesive without turning the Shared Kernel or registration slice into a second catalog service.

## Resolving Codes Through Their Owning Features

The new `GetDegreeByCodeQuery` gives registration a public way to verify that a well-formed degree code exists. The handler normalizes the code through the `Degree` value object and performs a no-tracking lookup in `ManageDegreesDbContext`. University resolution follows the same owner-query pattern, including an active-status check; rank codes are parsed from the rank catalog.

The command validator catches request-shape errors before dispatch. It checks the six-character employee number, required name, valid rank and extension number, at least one qualification, and duplicate degree codes. The handler then resolves each submitted code and reports unknown or inactive reference data as a field-specific validation failure rather than persisting caller-supplied labels as if they were authoritative.

The endpoint exposes `POST /api/academics/register`. A successful request returns `201 Created`; malformed or unresolved values return `400`; duplicate employee numbers and already-assigned extensions return `409`. The response derives access level from rank, so clients do not supply a second value that could disagree with the domain rule.

## Writing Once While Keeping Migration Ownership

The registration operation needs to create an academic and its qualifications while assigning an extension. Those changes must succeed or fail together. `RegisterAcademicDbContext` maps the three table shapes needed by the operation, but marks those tables `ExcludeFromMigrations()`. Its `SaveChangesAsync` persists the academic, qualifications, and extension assignment in one database unit of work; it does not become a competing schema owner.

Migration ownership stays with the feature that owns each table: Shared Kernel owns `Academics` and `AcademicQualifications`, ProvisionExtension owns `Extensions`, and the reference-data features own their catalogs. The migration-ownership matrix now distinguishes these owner contexts from the registration context that only maps existing tables. This is the important architectural split: a use case can coordinate multiple aggregates without claiming their schema.

The handler also accounts for races. A pre-check gives a clear duplicate-employee response, but another request could claim the same employee number or extension before the write. If SQL Server rejects the save, the handler clears tracked state and checks the specific conflicting values before translating the failure to `409`; unrelated database update failures are not silently reclassified.

## Aligning Domain Rules and Existing Schemas

The registration contract exposed mismatches between domain rules and persistence. Employee numbers are now trimmed, normalized to uppercase, and required to contain exactly six characters. An academic must have at least one qualification, and the domain rejects repeated degree codes to match the persisted `(EmpNr, DegreeCode)` key. The existing mutual exclusion between tenure and a contract end date remains enforced.

The migration work follows those rules without moving table ownership. Shared Kernel receives its baseline and an alignment migration; the latter renames the university-name column to the university-code representation while preserving existing values and applies the employee-number constraints. ManageDegrees receives its initial migration, and ProvisionExtension retains the assignment-length change under its own migration owner. `RegisterAcademicDbContext` receives no migration set of its own.

## Verifying the Route and Persistence Boundaries

I verified the feature through request validation, route behavior, fresh-context persistence checks, migration application, and failure-atomicity cases against SQL Server. The recorded results are:

| Area | Result |
| --- | ---: |
| RegisterAcademic | 49/49 tests passed |
| ManageDegrees | 24/24 tests passed |
| Shared Kernel | 34/34 tests passed |
| ProvisionExtension | 29/29 tests passed |
| Solution build | Passed |

The SQL Server tests use unique test databases, apply the owning migrations, read persisted data through fresh contexts, and clean up their databases. The existing development database was not modified because its Extensions schema did not match the current model; development configuration now targets a separate catalog. The ProvisionExtension test suite passes, while its mechanical gate still calls out two existing route-test gaps for the provision and deprovision endpoints. Those remain follow-up work rather than being folded into this registration slice.

## What's Next?

The next planned domain step is EP-2-2, `RecordQualification`. It can build on the persisted academic and qualification model while keeping the registration contract and migration ownership established here intact.

## Feedback Loop

Feedback is welcome. [Send your thoughts to john.miller@codemag.com](mailto:john.miller@codemag.com).

## Disclaimer

AI contributed to the writing of this post, but humans reviewed it, refined it, enhanced it, and gave it soul.

Prompts:

- create a blog post describing the Part-Twelve code changes
