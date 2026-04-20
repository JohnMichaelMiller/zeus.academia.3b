---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-24-project-overview-zeus-academia"
prompt: |
  Submit create-technology-instructions.prompt.md with arguments:
  technology_name: TypeScript (Frontend)
  technology_category: frontend
  primary_language: TypeScript
  project_context: Type-safe frontend development for Vue 3 SPA
  version_target: 5.3+
started: "2026-02-24T00:45:00Z"
ended: "2026-02-24T00:55:00Z"
task_durations:
  - task: "analyze TypeScript patterns"
    duration: "00:03:00"
  - task: "document type standards"
    duration: "00:05:00"
  - task: "create examples"
    duration: "00:02:00"
total_duration: "00:10:00"
ai_log: "ai-logs/2026/02/24/2026-02-24-project-overview-zeus-academia/conversation.md"
source: ".github/prompts/create-technology-instructions.prompt.md"
name: "TypeScript Frontend Standards"
description: "TypeScript coding standards for Vue 3 feature-domain implementation"
applyTo: "src/**/*.ts"
tags: [typescript, frontend, type-safety, vue3]
---

# TypeScript Frontend Standards

**Role**: Type-safe frontend development for Vue 3 SPA
**Version**: 5.3+
**Language**: TypeScript

## Core Principles

- **Strict Mode**: Enable all strict type-checking options
- **No Implicit Any**: Explicitly type all variables and functions
- **Type Inference**: Let TypeScript infer when obvious
- **Interfaces over Types**: Prefer interfaces for object shapes
- **Immutability**: Use `readonly` and `Readonly<T>` where appropriate

## File Organization

- `src/features/<Feature>/Shared/` - Feature-scoped type definitions shared across related use-cases
- `src/features/<Feature>/<UseCase>/` - Keep use-case private request, response, and view-model types close to the code that uses them
- `src/shared/types/` - Cross-cutting utility types and application-wide contracts
- `src/shared/types/enums.ts` - Shared enums and constants
- Naming: PascalCase for types/interfaces, camelCase for files

## Standard Patterns

### Type Definitions

```typescript
// src/features/students/shared/student.types.ts
export interface Student {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  dateOfBirth: Date;
  enrollmentDate: Date;
  status: StudentStatus;
  gpa?: number;
}

export type StudentStatus = "active" | "inactive" | "graduated" | "suspended";

export interface StudentFilters {
  status?: StudentStatus;
  searchTerm?: string;
  minGpa?: number;
  maxGpa?: number;
}

export type StudentCreatePayload = Omit<Student, "id" | "status">;
export type StudentUpdatePayload = Partial<Omit<Student, "id">>;

// Readonly variant for immutable data
export type ReadonlyStudent = Readonly<Student>;
```

**Usage**: Define clear types for domain models
**Avoid**: Using `any`, inline types, or `object` type

### API Types

```typescript
// src/shared/types/api/responses.ts
export interface ApiResponse<T> {
  data: T;
  status: number;
  message?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  hasNextPage: boolean;
}

export interface ApiError {
  code: string;
  message: string;
  details?: Record<string, string[]>;
}

// src/features/students/shared/studentApi.types.ts
export interface GetStudentsRequest {
  page?: number;
  pageSize?: number;
  status?: StudentStatus;
  search?: string;
}

export type GetStudentsResponse = PaginatedResponse<Student>;
export type GetStudentResponse = ApiResponse<Student>;
export type CreateStudentResponse = ApiResponse<Student>;
```

**Usage**: Type all API interactions
**Avoid**: Untyped fetch/axios calls

### Type Guards

```typescript
// src/features/students/shared/guards.ts
export function isStudent(value: unknown): value is Student {
  return (
    typeof value === "object" &&
    value !== null &&
    "id" in value &&
    "firstName" in value &&
    "lastName" in value &&
    "email" in value
  );
}

export function isApiError(error: unknown): error is ApiError {
  return (
    typeof error === "object" &&
    error !== null &&
    "code" in error &&
    "message" in error
  );
}

// Usage in error handling
try {
  await studentApi.create(data);
} catch (error) {
  if (isApiError(error)) {
    console.error(`API Error [${error.code}]: ${error.message}`);
  } else {
    console.error("Unknown error:", error);
  }
}
```

**Usage**: Narrow unknown types safely
**Avoid**: Type assertions (`as`) without validation

### Generic Utilities

```typescript
// src/shared/types/utils.ts
export type Nullable<T> = T | null;
export type Optional<T> = T | undefined;
export type DeepPartial<T> = {
  [P in keyof T]?: T[P] extends object ? DeepPartial<T[P]> : T[P];
};
export type RequireAtLeastOne<T, Keys extends keyof T = keyof T> = Pick<
  T,
  Exclude<keyof T, Keys>
> &
  {
    [K in Keys]-?: Required<Pick<T, K>> & Partial<Pick<T, Exclude<Keys, K>>>;
  }[Keys];

// Usage
type StudentUpdate = DeepPartial<Student>;
type StudentFilter = RequireAtLeastOne<StudentFilters, "status" | "searchTerm">;
```

**Usage**: Create reusable type transformations
**Avoid**: Repeating complex type patterns

### Enum vs Union Types

```typescript
// Prefer union types for simple cases
export type CourseStatus = "draft" | "published" | "archived";

// Use enums for values with specific meanings or when mapping is needed
export enum UserRole {
  Student = "STUDENT",
  Instructor = "INSTRUCTOR",
  Admin = "ADMIN",
}

export const UserRoleLabels: Record<UserRole, string> = {
  [UserRole.Student]: "Student",
  [UserRole.Instructor]: "Instructor",
  [UserRole.Admin]: "Administrator",
};
```

**Usage**: Union types for simple string literals, enums for structured values
**Avoid**: Numeric enums (use string enums)

### Function Types

```typescript
// src/shared/types/functions.ts
export type AsyncFunction<T = void> = () => Promise<T>;
export type EventHandler<T = Event> = (event: T) => void;
export type Validator<T> = (value: T) => boolean | string;
export type Transformer<TInput, TOutput> = (input: TInput) => TOutput;

// Usage in component
interface Props {
  onSubmit: AsyncFunction<boolean>;
  validator: Validator<string>;
  transformer: Transformer<string, number>;
}
```

**Usage**: Type callbacks and functions passed as props
**Avoid**: Untyped function parameters

### Discriminated Unions

```typescript
// src/shared/types/api/results.ts
export type ApiResult<T, E = ApiError> =
  | { success: true; data: T }
  | { success: false; error: E };

// Usage
async function fetchStudent(id: string): Promise<ApiResult<Student>> {
  try {
    const response = await studentApi.getById(id);
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error as ApiError,
    };
  }
}

// Component usage
const result = await fetchStudent(studentId.value);
if (result.success) {
  // TypeScript knows result.data exists
  console.log(result.data.firstName);
} else {
  // TypeScript knows result.error exists
  console.error(result.error.message);
}
```

**Usage**: Type-safe result handling without throwing
**Avoid**: Relying solely on try/catch for error handling types

## Vue 3 Integration

### Component Props

```typescript
// Prefer interface over type for props
interface StudentCardProps {
  student: Student;
  showActions?: boolean;
  onEdit?: (student: Student) => void;
  onDelete?: (id: string) => void;
}

const props = withDefaults(defineProps<StudentCardProps>(), {
  showActions: true,
});
```

### Composable Return Types

```typescript
// composables/useStudentEnrollment.ts
export interface UseStudentEnrollmentReturn {
  enrollments: ComputedRef<Enrollment[]>;
  activeEnrollments: ComputedRef<Enrollment[]>;
  loading: Ref<boolean>;
  error: Ref<Error | null>;
  loadEnrollments: () => Promise<void>;
  enroll: (courseId: string) => Promise<void>;
}

export function useStudentEnrollment(
  studentId: Ref<string>,
): UseStudentEnrollmentReturn {
  // Implementation
}
```

### Store Types

```typescript
// stores/useStudentStore.ts
import type { Ref, ComputedRef } from "vue";

export interface StudentStore {
  // State
  students: Ref<Student[]>;
  selectedStudent: Ref<Student | null>;
  loading: Ref<boolean>;
  error: Ref<Error | null>;
  // Getters
  activeStudents: ComputedRef<Student[]>;
  studentCount: ComputedRef<number>;
  // Actions
  fetchStudents: () => Promise<void>;
  createStudent: (data: StudentCreatePayload) => Promise<Student>;
  updateStudent: (id: string, data: StudentUpdatePayload) => Promise<Student>;
  deleteStudent: (id: string) => Promise<void>;
}
```

## TSConfig Settings

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "module": "ESNext",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "skipLibCheck": true,

    /* Bundler */
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "preserve",

    /* Linting */
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,
    "noImplicitReturns": true,
    "forceConsistentCasingInFileNames": true,

    /* Path Mapping */
    "baseUrl": ".",
    "paths": {
      "@/*": ["src/*"]
    }
  },
  "include": ["src/**/*.ts", "src/**/*.vue"],
  "exclude": ["node_modules", "dist"]
}
```

## Validation Checklist

- [ ] `strict: true` in tsconfig.json
- [ ] No `any` types (use `unknown` if type unknown)
- [ ] All functions have return types (or inferred)
- [ ] Interface names in PascalCase
- [ ] Type guards for runtime validation
- [ ] Discriminated unions for complex states
- [ ] Generic types parameterized appropriately
- [ ] API types match backend contracts
- [ ] Readonly for immutable data

## Anti-Patterns

❌ Using `any` type
✅ Use `unknown` and type guards

❌ Type assertions without validation (`as Student`)
✅ Type guards (`isStudent(value)`)

❌ Optional chaining everywhere (`obj?.prop?.nested?.value`)
✅ Proper null handling with type narrowing

❌ Inline complex types
✅ Extract to named types/interfaces

❌ Ignoring TypeScript errors (`@ts-ignore`)
✅ Fix the underlying type issue

❌ Numeric enums
✅ String enums or union types

❌ `Function` or `Object` types
✅ Specific function signatures and object shapes

❌ Mutable type definitions for immutable data
✅ `Readonly<T>` or `readonly` modifier
