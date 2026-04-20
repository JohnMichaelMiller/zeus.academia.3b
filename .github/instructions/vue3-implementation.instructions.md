---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-24-project-overview-zeus-academia"
prompt: |
  Submit create-technology-instructions.prompt.md with arguments:
  technology_name: Vue 3
  technology_category: frontend
  primary_language: TypeScript
  project_context: SPA frontend for Academic Management System
  version_target: 3.4+
started: "2026-02-24T00:25:00Z"
ended: "2026-02-24T00:35:00Z"
task_durations:
  - task: "analyze Vue 3 patterns"
    duration: "00:03:00"
  - task: "document standards"
    duration: "00:05:00"
  - task: "create examples"
    duration: "00:02:00"
total_duration: "00:10:00"
ai_log: "ai-logs/2026/02/24/2026-02-24-project-overview-zeus-academia/conversation.md"
source: ".github/prompts/create-technology-instructions.prompt.md"
name: "Vue 3 Standards"
description: "Vue 3 coding standards and best practices for feature-domain UI implementation"
applyTo: "src/**/*.{vue,ts}"
tags: [vue3, frontend, typescript, composition-api, sfc]
---

# Vue 3 Implementation Standards

**Role**: SPA frontend framework for Academic Management System
**Version**: 3.4+
**Language**: TypeScript

## Core Principles

- **Composition API**: Use exclusively; avoid Options API
- **TypeScript**: Strict typing for all props, emits, and composables
- **SFC**: Single File Components with `<script setup>` syntax
- **Reactivity**: Prefer `ref()` for primitives, `reactive()` for objects
- **Performance**: Lazy load routes and heavy components

## File Organization

- `src/features/<Feature>/<UseCase>/` - Co-locate page components, composables, stores, route modules, and UI-facing types for one use-case
- `src/features/<Feature>/Shared/` - Feature-scoped Vue components or composables reused within the same feature domain
- `src/shared/components/` - Cross-cutting UI components
- `src/shared/composables/` - Cross-cutting composition functions
- `src/shared/router/` - Router bootstrap and global guards
- Naming: `ComponentName.vue`, `useFeatureName.ts` (composables)

## Standard Patterns

### Component Structure

```vue
<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import type { PropType } from "vue";

interface Props {
  userId: string;
  isActive?: boolean;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  update: [value: string];
  close: [];
}>();

// Local state
const loading = ref(false);
const userData = ref<User | null>(null);

// Computed properties
const displayName = computed(() =>
  userData.value
    ? `${userData.value.firstName} ${userData.value.lastName}`
    : "",
);

// Methods
const loadUser = async () => {
  loading.value = true;
  try {
    userData.value = await fetchUser(props.userId);
  } finally {
    loading.value = false;
  }
};

// Lifecycle
onMounted(() => {
  loadUser();
});

// Expose for parent access (optional)
defineExpose({
  refresh: loadUser,
});
</script>

<template>
  <div class="user-profile">
    <template v-if="loading">
      <LoadingSpinner />
    </template>
    <template v-else-if="userData">
      <h2>{{ displayName }}</h2>
      <button @click="emit('update', userData.id)">Update</button>
    </template>
  </div>
</template>

<style scoped>
.user-profile {
  /* Component-specific styles */
}
</style>
```

**Usage**: Standard component with props, emits, and async data
**Avoid**: Options API, any types, non-TypeScript props

### Composable Pattern

```typescript
// src/features/enrollment/listStudentEnrollments/useStudentEnrollment.ts
import { ref, computed } from "vue";
import type { Ref } from "vue";
import { useEnrollmentStore } from "./useEnrollmentStore";

export interface UseStudentEnrollmentOptions {
  autoLoad?: boolean;
}

export function useStudentEnrollment(
  studentId: Ref<string>,
  options: UseStudentEnrollmentOptions = {},
) {
  const store = useEnrollmentStore();
  const loading = ref(false);
  const error = ref<Error | null>(null);

  const enrollments = computed(() =>
    store.getEnrollmentsByStudent(studentId.value),
  );

  const activeEnrollments = computed(() =>
    enrollments.value.filter((e) => e.status === "active"),
  );

  async function loadEnrollments() {
    loading.value = true;
    error.value = null;
    try {
      await store.fetchEnrollments(studentId.value);
    } catch (e) {
      error.value = e as Error;
    } finally {
      loading.value = false;
    }
  }

  async function enroll(courseId: string) {
    await store.enrollStudent(studentId.value, courseId);
  }

  if (options.autoLoad) {
    loadEnrollments();
  }

  return {
    enrollments,
    activeEnrollments,
    loading,
    error,
    loadEnrollments,
    enroll,
  };
}
```

**Usage**: Extract reusable component logic
**Avoid**: Global state in composables (use Pinia instead)

### Props Validation

```typescript
// Using TypeScript interface (preferred)
interface Props {
  title: string;
  count: number;
  items: Array<{ id: string; name: string }>;
  status?: "active" | "inactive";
}

const props = withDefaults(defineProps<Props>(), {
  status: "active",
});

// With runtime validation (when needed)
const props = defineProps({
  title: {
    type: String,
    required: true,
  },
  count: {
    type: Number,
    required: true,
    validator: (value: number) => value >= 0,
  },
  status: {
    type: String as PropType<"active" | "inactive">,
    default: "active",
  },
});
```

**Usage**: Always type props; use runtime validation for complex constraints
**Avoid**: Untyped props, missing required flags

### Template Refs

```typescript
const inputRef = ref<HTMLInputElement | null>(null);
const componentRef = ref<InstanceType<typeof MyComponent> | null>(null);

onMounted(() => {
  inputRef.value?.focus();
  componentRef.value?.refresh();
});
```

```vue
<template>
  <input ref="inputRef" type="text" />
  <MyComponent ref="componentRef" />
</template>
```

**Usage**: Accessing DOM elements or child component methods
**Avoid**: Overusing refs; prefer props/emits for component communication

## Integration

- **Pinia**: Import stores from the same use-case folder first, then promote to `src/features/<Feature>/Shared/` or `src/shared/` only when reuse is proven
- **Router**: Use `useRouter()` and `useRoute()` for navigation
- **API**: Axios instance from `@/services/api`
- **Auth**: Azure AD B2C via `@/shared/composables/useAuth`
- **Validation**: Vuelidate for form validation (frontend-only)

## Performance Optimization

- **Lazy Loading**: Routes and heavy components

```typescript
// src/shared/router/index.ts
const routes = [
  {
    path: "/students",
    component: () => import("@/features/students/listStudents/StudentList.vue"),
  },
];

// In component
const HeavyChart = defineAsyncComponent(
  () => import("@/shared/components/HeavyChart.vue"),
);
```

- **v-once**: Static content that never changes
- **v-memo**: Expensive list items with stable data
- **Computed vs Methods**: Use computed for derived state
- **Event Handlers**: Use function references, not inline arrows (when possible)

```vue
<!-- Good: stable reference -->
<button @click="handleClick">Click</button>

<!-- Avoid: creates new function each render -->
<button @click="() => doSomething(id)">Click</button>

<!-- Alternative: use computed or bind -->
<button @click="handleClickWithId">Click</button>
```

## Lifecycle Hooks

- `onMounted()`: DOM ready, initial data fetch
- `onUnmounted()`: Cleanup (event listeners, timers, subscriptions)
- `onBeforeUpdate()`: Before DOM update
- `onUpdated()`: After DOM update (use sparingly)
- `watch()`: Reactive side effects
- `watchEffect()`: Automatic dependency tracking

```typescript
// Clean up subscriptions
const unsubscribe = ref<(() => void) | null>(null);

onMounted(() => {
  unsubscribe.value = store.$onAction((action) => {
    console.log(action);
  });
});

onUnmounted(() => {
  unsubscribe.value?.();
});
```

## Validation Checklist

- [ ] Use `<script setup lang="ts">` exclusively
- [ ] All props typed with TypeScript interfaces
- [ ] Emits declared with `defineEmits<{}>()`
- [ ] Composables prefixed with `use` and return typed objects
- [ ] Components named PascalCase
- [ ] Styles scoped to component
- [ ] Async operations with loading and error states
- [ ] Cleanup in `onUnmounted` for subscriptions/listeners
- [ ] Lazy load routes
- [ ] No `any` types

## Anti-Patterns

❌ Options API usage
✅ Composition API with `<script setup>`

❌ Untyped props (`defineProps(['title', 'count'])`)
✅ TypeScript interface props (`defineProps<Props>()`)

❌ Mutating props directly
✅ Emit events to parent for updates

❌ Global state in composables
✅ Pinia stores for shared state

❌ `v-for` without `:key`
✅ Unique `:key` for each list item

❌ Inline event handler creation in loops
✅ Stable function references

❌ Accessing `$refs` in template
✅ Template refs with TypeScript types

❌ Deep watchers without cleanup
✅ `onUnmounted` cleanup for watchers
