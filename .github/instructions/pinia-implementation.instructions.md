---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-24-project-overview-zeus-academia"
prompt: |
  Submit create-technology-instructions.prompt.md with arguments:
  technology_name: Pinia
  technology_category: frontend
  primary_language: TypeScript
  project_context: State management for Vue 3 SPA
  version_target: 2.1+
started: "2026-02-24T00:35:00Z"
ended: "2026-02-24T00:45:00Z"
task_durations:
  - task: "analyze Pinia patterns"
    duration: "00:03:00"
  - task: "document store standards"
    duration: "00:05:00"
  - task: "create examples"
    duration: "00:02:00"
total_duration: "00:10:00"
ai_log: "ai-logs/2026/02/24/2026-02-24-project-overview-zeus-academia/conversation.md"
source: ".github/prompts/create-technology-instructions.prompt.md"
name: "Pinia Standards"
description: "Pinia state management standards and best practices for Vue 3 application"
applyTo: "src/frontend/stores/**/*.ts"
tags: [pinia, state-management, frontend, typescript, vue3]
---

# Pinia State Management Standards

**Role**: Global state management for Vue 3 SPA
**Version**: 2.1+
**Language**: TypeScript

## Core Principles

- **Composition Stores**: Use setup stores (not options stores)
- **TypeScript**: Full type safety for state, getters, and actions
- **Modularity**: One store per domain/feature
- **Naming**: `use<Feature>Store` pattern
- **Reactivity**: Leverage Vue 3 reactivity system

## File Organization

- `src/frontend/stores/` - All Pinia stores
- `src/frontend/stores/index.ts` - Pinia instance configuration
- Naming: `useEnrollmentStore.ts`, `useAuthStore.ts`, `useStudentStore.ts`
- One store per file

## Standard Patterns

### Store Structure (Setup Style)

```typescript
// stores/useStudentStore.ts
import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { Student, StudentFilters } from '@/types/student'
import { studentApi } from '@/services/api/students'

export const useStudentStore = defineStore('student', () => {
  // State
  const students = ref<Student[]>([])
  const selectedStudent = ref<Student | null>(null)
  const loading = ref(false)
  const error = ref<Error | null>(null)
  const filters = ref<StudentFilters>({
    status: 'active',
    searchTerm: ''
  })
  
  // Getters (computed)
  const activeStudents = computed(() =>
    students.value.filter(s => s.status === 'active')
  )
  
  const filteredStudents = computed(() => {
    let result = students.value
    
    if (filters.value.status) {
      result = result.filter(s => s.status === filters.value.status)
    }
    
    if (filters.value.searchTerm) {
      const term = filters.value.searchTerm.toLowerCase()
      result = result.filter(s =>
        s.firstName.toLowerCase().includes(term) ||
        s.lastName.toLowerCase().includes(term) ||
        s.email.toLowerCase().includes(term)
      )
    }
    
    return result
  })
  
  const studentCount = computed(() => students.value.length)
  
  // Actions
  async function fetchStudents() {
    loading.value = true
    error.value = null
    try {
      const response = await studentApi.getAll()
      students.value = response.data
    } catch (e) {
      error.value = e as Error
      throw e
    } finally {
      loading.value = false
    }
  }
  
  async function fetchStudent(id: string) {
    loading.value = true
    error.value = null
    try {
      const response = await studentApi.getById(id)
      selectedStudent.value = response.data
      return response.data
    } catch (e) {
      error.value = e as Error
      throw e
    } finally {
      loading.value = false
    }
  }
  
  async function createStudent(data: Omit<Student, 'id'>) {
    loading.value = true
    error.value = null
    try {
      const response = await studentApi.create(data)
      students.value.push(response.data)
      return response.data
    } catch (e) {
      error.value = e as Error
      throw e
    } finally {
      loading.value = false
    }
  }
  
  async function updateStudent(id: string, data: Partial<Student>) {
    loading.value = true
    error.value = null
    try {
      const response = await studentApi.update(id, data)
      const index = students.value.findIndex(s => s.id === id)
      if (index !== -1) {
        students.value[index] = response.data
      }
      if (selectedStudent.value?.id === id) {
        selectedStudent.value = response.data
      }
      return response.data
    } catch (e) {
      error.value = e as Error
      throw e
    } finally {
      loading.value = false
    }
  }
  
  async function deleteStudent(id: string) {
    loading.value = true
    error.value = null
    try {
      await studentApi.delete(id)
      students.value = students.value.filter(s => s.id !== id)
      if (selectedStudent.value?.id === id) {
        selectedStudent.value = null
      }
    } catch (e) {
      error.value = e as Error
      throw e
    } finally {
      loading.value = false
    }
  }
  
  function setFilters(newFilters: Partial<StudentFilters>) {
    filters.value = { ...filters.value, ...newFilters }
  }
  
  function clearFilters() {
    filters.value = {
      status: 'active',
      searchTerm: ''
    }
  }
  
  function selectStudent(student: Student | null) {
    selectedStudent.value = student
  }
  
  function $reset() {
    students.value = []
    selectedStudent.value = null
    loading.value = false
    error.value = null
    clearFilters()
  }
  
  return {
    // State
    students,
    selectedStudent,
    loading,
    error,
    filters,
    // Getters
    activeStudents,
    filteredStudents,
    studentCount,
    // Actions
    fetchStudents,
    fetchStudent,
    createStudent,
    updateStudent,
    deleteStudent,
    setFilters,
    clearFilters,
    selectStudent,
    $reset
  }
})
```

**Usage**: Standard CRUD store with filtering and selection
**Avoid**: Options API stores, direct state mutation from components

### Store Composition

```typescript
// stores/useEnrollmentStore.ts
import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { useStudentStore } from './useStudentStore'
import { useCourseStore } from './useCourseStore'
import type { Enrollment } from '@/types/enrollment'

export const useEnrollmentStore = defineStore('enrollment', () => {
  const studentStore = useStudentStore()
  const courseStore = useCourseStore()
  
  const enrollments = ref<Enrollment[]>([])
  
  const enrichedEnrollments = computed(() =>
    enrollments.value.map(enrollment => ({
      ...enrollment,
      student: studentStore.students.find(s => s.id === enrollment.studentId),
      course: courseStore.courses.find(c => c.id === enrollment.courseId)
    }))
  )
  
  async function enrollStudent(studentId: string, courseId: string) {
    // Implementation
  }
  
  return {
    enrollments,
    enrichedEnrollments,
    enrollStudent
  }
})
```

**Usage**: Compose multiple stores for related functionality
**Avoid**: Circular dependencies between stores

### Persisted State

```typescript
// stores/useAuthStore.ts
import { ref, computed } from 'vue'
import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('auth_token'))
  const user = ref<User | null>(null)
  
  const isAuthenticated = computed(() => !!token.value && !!user.value)
  
  async function login(credentials: LoginCredentials) {
    const response = await authApi.login(credentials)
    token.value = response.token
    user.value = response.user
    localStorage.setItem('auth_token', response.token)
  }
  
  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('auth_token')
  }
  
  return {
    token,
    user,
    isAuthenticated,
    login,
    logout
  }
}, {
  // Alternative: use pinia-plugin-persistedstate
  persist: false // Handle manually for sensitive data
})
```

**Usage**: Manual persistence for auth tokens and user preferences
**Avoid**: Persisting entire store; be selective with sensitive data

### Store Actions with Optimistic Updates

```typescript
async function updateStudentStatus(id: string, status: StudentStatus) {
  const student = students.value.find(s => s.id === id)
  if (!student) return
  
  // Optimistic update
  const originalStatus = student.status
  student.status = status
  
  try {
    await studentApi.updateStatus(id, status)
  } catch (e) {
    // Rollback on error
    student.status = originalStatus
    error.value = e as Error
    throw e
  }
}
```

**Usage**: Improve perceived performance for user actions
**Avoid**: Optimistic updates for critical operations or complex state changes

## Integration

- **Vue Components**: Import with `const store = useXxxStore()`
- **Composables**: Access stores within composables
- **Router Guards**: Use stores for auth checks and data preloading
- **API Services**: Call API functions from store actions
- **Plugins**: Configure in `main.ts` with `app.use(createPinia())`

```typescript
// main.ts
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.mount('#app')
```

## Testing Patterns

```typescript
// __tests__/stores/useStudentStore.spec.ts
import { setActivePinia, createPinia } from 'pinia'
import { useStudentStore } from '@/stores/useStudentStore'
import { vi } from 'vitest'

describe('useStudentStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })
  
  it('fetches students', async () => {
    const store = useStudentStore()
    
    // Mock API
    vi.mock('@/services/api/students', () => ({
      studentApi: {
        getAll: vi.fn().mockResolvedValue({
          data: [{ id: '1', name: 'John Doe' }]
        })
      }
    }))
    
    await store.fetchStudents()
    
    expect(store.students).toHaveLength(1)
    expect(store.loading).toBe(false)
  })
  
  it('filters students by search term', () => {
    const store = useStudentStore()
    store.students = [
      { id: '1', firstName: 'John', lastName: 'Doe', email: 'john@example.com' },
      { id: '2', firstName: 'Jane', lastName: 'Smith', email: 'jane@example.com' }
    ]
    
    store.setFilters({ searchTerm: 'john' })
    
    expect(store.filteredStudents).toHaveLength(1)
    expect(store.filteredStudents[0].firstName).toBe('John')
  })
})
```

## Validation Checklist

- [ ] Store name matches `use<Feature>Store` pattern
- [ ] Setup stores (not options stores)
- [ ] All state typed with TypeScript
- [ ] Getters use `computed()`
- [ ] Actions async where needed
- [ ] Loading/error states for async operations
- [ ] $reset() function for cleanup
- [ ] No direct state mutations from components
- [ ] Appropriate use of store composition
- [ ] Sensitive data handled securely (auth tokens)

## Anti-Patterns

❌ Options API stores
✅ Setup stores with composition API

❌ Mutating store state directly in components
✅ Call store actions to modify state

❌ Storing derived state
✅ Use computed getters for derived values

❌ Mixing unrelated concerns in one store
✅ Separate stores per domain/feature

❌ Async logic in getters
✅ Async logic only in actions

❌ Untyped state or actions
✅ Full TypeScript typing

❌ Persisting entire store including loading states
✅ Selective persistence of relevant data

❌ Creating store instances outside setup
✅ Create stores within component setup or composables
