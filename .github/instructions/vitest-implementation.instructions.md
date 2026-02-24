---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-24-project-overview-zeus-academia"
prompt: |
  Submit create-technology-instructions.prompt.md with arguments:
  technology_name: Vitest
  technology_category: testing
  primary_language: TypeScript
  project_context: Unit and component testing for Vue 3 frontend
  version_target: 1.2+
started: "2026-02-24T01:35:00Z"
ended: "2026-02-24T01:45:00Z"
task_durations:
  - task: "analyze testing patterns"
    duration: "00:03:00"
  - task: "document test standards"
    duration: "00:05:00"
  - task: "create examples"
    duration: "00:02:00"
total_duration: "00:10:00"
ai_log: "ai-logs/2026/02/24/2026-02-24-project-overview-zeus-academia/conversation.md"
source: ".github/prompts/create-technology-instructions.prompt.md"
name: "Vitest Testing Standards"
description: "Vitest testing standards for Vue 3 frontend application"
applyTo: "src/frontend/**/*.{test,spec}.ts"
tags: [vitest, testing, typescript, frontend, vue3, unit-tests]
---

# Vitest Testing Standards

**Role**: Unit and component testing for Vue 3 frontend
**Version**: 1.2+
**Language**: TypeScript
**Target Coverage**: 80%+

## Core Principles

- **Arrange-Act-Assert**: Clear test structure
- **Fast Execution**: Tests should run in milliseconds
- **Isolated Tests**: No shared state between tests
- **Mock External Dependencies**: API calls, stores, external libraries
- **Component Testing**: Use Testing Library patterns for Vue components

## File Organization

- `src/frontend/**/__tests__/` - Test files in `__tests__` folders
- `src/frontend/**/*.test.ts` - Unit test files alongside source
- `src/frontend/**/*.spec.ts` - Component spec files
- Naming: `<filename>.test.ts` or `<ComponentName>.spec.ts`

## Standard Patterns

### Unit Test Structure

```typescript
// composables/__tests__/useStudentEnrollment.test.ts
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { useStudentEnrollment } from '../useStudentEnrollment'
import { useEnrollmentStore } from '@/stores/useEnrollmentStore'
import { ref } from 'vue'

vi.mock('@/stores/useEnrollmentStore')

describe('useStudentEnrollment', () => {
  const mockStore = {
    getEnrollmentsByStudent: vi.fn(),
    fetchEnrollments: vi.fn(),
    enrollStudent: vi.fn()
  }

  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(useEnrollmentStore).mockReturnValue(mockStore as any)
  })

  describe('enrollments', () => {
    it('returns enrollments from store', () => {
      // Arrange
      const studentId = ref('student-123')
      const mockEnrollments = [
        { id: '1', studentId: 'student-123', courseId: 'course-1', status: 'active' }
      ]
      mockStore.getEnrollmentsByStudent.mockReturnValue(mockEnrollments)

      // Act
      const { enrollments } = useStudentEnrollment(studentId)

      // Assert
      expect(enrollments.value).toEqual(mockEnrollments)
      expect(mockStore.getEnrollmentsByStudent).toHaveBeenCalledWith('student-123')
    })
  })

  describe('loadEnrollments', () => {
    it('fetches enrollments and sets loading state', async () => {
      // Arrange
      const studentId = ref('student-123')
      mockStore.fetchEnrollments.mockResolvedValue(undefined)

      // Act
      const { loading, loadEnrollments } = useStudentEnrollment(studentId)
      
      expect(loading.value).toBe(false)
      const promise = loadEnrollments()
      expect(loading.value).toBe(true)
      await promise

      // Assert
      expect(loading.value).toBe(false)
      expect(mockStore.fetchEnrollments).toHaveBeenCalledWith('student-123')
    })

    it('sets error state when fetch fails', async () => {
      // Arrange
      const studentId = ref('student-123')
      const mockError = new Error('Network error')
      mockStore.fetchEnrollments.mockRejectedValue(mockError)

      // Act
      const { error, loadEnrollments } = useStudentEnrollment(studentId)
      await loadEnrollments()

      // Assert
      expect(error.value).toBe(mockError)
    })
  })
})
```

### Component Testing with @testing-library/vue

```typescript
// components/__tests__/StudentCard.spec.ts
import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/vue'
import StudentCard from '../StudentCard.vue'
import type { Student } from '@/types/models/student'

describe('StudentCard', () => {
  const mockStudent: Student = {
    id: 'student-123',
    firstName: 'John',
    lastName: 'Doe',
    email: 'john.doe@example.com',
    status: 'active',
    dateOfBirth: new Date('2000-01-01'),
    enrollmentDate: new Date('2020-09-01')
  }

  it('renders student information', () => {
    // Arrange & Act
    render(StudentCard, {
      props: {
        student: mockStudent
      }
    })

    // Assert
    expect(screen.getByText('John Doe')).toBeInTheDocument()
    expect(screen.getByText('john.doe@example.com')).toBeInTheDocument()
  })

  it('emits edit event when edit button is clicked', async () => {
    // Arrange
    const { emitted } = render(StudentCard, {
      props: {
        student: mockStudent,
        showActions: true
      }
    })

    // Act
    const editButton = screen.getByRole('button', { name: /edit/i })
    await fireEvent.click(editButton)

    // Assert
    expect(emitted()).toHaveProperty('edit')
    expect(emitted().edit[0]).toEqual([mockStudent])
  })

  it('does not render actions when showActions is false', () => {
    // Arrange & Act
    render(StudentCard, {
      props: {
        student: mockStudent,
        showActions: false
      }
    })

    // Assert
    expect(screen.queryByRole('button', { name: /edit/i })).not.toBeInTheDocument()
    expect(screen.queryByRole('button', { name: /delete/i })).not.toBeInTheDocument()
  })

  it('displays status badge with correct color', () => {
    // Arrange & Act
    render(StudentCard, {
      props: {
        student: { ...mockStudent, status: 'inactive' }
      }
    })

    // Assert
    const statusBadge = screen.getByText('inactive')
    expect(statusBadge).toHaveClass('badge-inactive')
  })
})
```

### Testing Pinia Stores

```typescript
// stores/__tests__/useStudentStore.test.ts
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useStudentStore } from '../useStudentStore'
import { studentApi } from '@/services/api/students'
import type { Student } from '@/types/models/student'

vi.mock('@/services/api/students')

describe('useStudentStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('fetchStudents', () => {
    it('fetches students and updates state', async () => {
      // Arrange
      const store = useStudentStore()
      const mockStudents: Student[] = [
        { id: '1', firstName: 'John', lastName: 'Doe', email: 'john@example.com' }
      ]
      vi.mocked(studentApi.getAll).mockResolvedValue({ data: mockStudents })

      // Act
      await store.fetchStudents()

      // Assert
      expect(store.students).toEqual(mockStudents)
      expect(store.loading).toBe(false)
      expect(store.error).toBeNull()
    })

    it('sets error state when fetch fails', async () => {
      // Arrange
      const store = useStudentStore()
      const mockError = new Error('API Error')
      vi.mocked(studentApi.getAll).mockRejectedValue(mockError)

      // Act
      await expect(store.fetchStudents()).rejects.toThrow('API Error')

      // Assert
      expect(store.error).toBe(mockError)
      expect(store.loading).toBe(false)
    })
  })

  describe('filteredStudents', () => {
    it('filters students by search term', () => {
      // Arrange
      const store = useStudentStore()
      store.students = [
        { id: '1', firstName: 'John', lastName: 'Doe', email: 'john@example.com', status: 'active' },
        { id: '2', firstName: 'Jane', lastName: 'Smith', email: 'jane@example.com', status: 'active' }
      ]

      // Act
      store.setFilters({ searchTerm: 'john' })

      // Assert
      expect(store.filteredStudents).toHaveLength(1)
      expect(store.filteredStudents[0].firstName).toBe('John')
    })

    it('filters students by status', () => {
      // Arrange
      const store = useStudentStore()
      store.students = [
        { id: '1', firstName: 'John', lastName: 'Doe', status: 'active' },
        { id: '2', firstName: 'Jane', lastName: 'Smith', status: 'inactive' }
      ]

      // Act
      store.setFilters({ status: 'inactive' })

      // Assert
      expect(store.filteredStudents).toHaveLength(1)
      expect(store.filteredStudents[0].status).toBe('inactive')
    })
  })
})
```

### Mocking API Calls

```typescript
// services/__tests__/api.test.ts
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { studentApi } from '../api/students'
import axios from 'axios'

vi.mock('axios')

describe('studentApi', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('getAll', () => {
    it('fetches all students', async () => {
      // Arrange
      const mockResponse = {
        data: [{ id: '1', firstName: 'John', lastName: 'Doe' }]
      }
      vi.mocked(axios.get).mockResolvedValue(mockResponse)

      // Act
      const result = await studentApi.getAll()

      // Assert
      expect(axios.get).toHaveBeenCalledWith('/api/students')
      expect(result).toEqual(mockResponse)
    })
  })

  describe('create', () => {
    it('posts student data', async () => {
      // Arrange
      const newStudent = {
        firstName: 'John',
        lastName: 'Doe',
        email: 'john@example.com'
      }
      const mockResponse = {
        data: { id: '1', ...newStudent }
      }
      vi.mocked(axios.post).mockResolvedValue(mockResponse)

      // Act
      const result = await studentApi.create(newStudent)

      // Assert
      expect(axios.post).toHaveBeenCalledWith('/api/students', newStudent)
      expect(result.data.id).toBe('1')
    })
  })
})
```

### Testing Async Components

```typescript
// views/__tests__/StudentList.spec.ts
import { describe, it, expect, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/vue'
import StudentList from '../StudentList.vue'
import { useStudentStore } from '@/stores/useStudentStore'

vi.mock('@/stores/useStudentStore')

describe('StudentList', () => {
  it('displays loading state while fetching', async () => {
    // Arrange
    const mockStore = {
      students: [],
      loading: true,
      fetchStudents: vi.fn()
    }
    vi.mocked(useStudentStore).mockReturnValue(mockStore as any)

    // Act
    render(StudentList)

    // Assert
    expect(screen.getByText(/loading/i)).toBeInTheDocument()
  })

  it('displays students after loading', async () => {
    // Arrange
    const mockStudents = [
      { id: '1', firstName: 'John', lastName: 'Doe', email: 'john@example.com' }
    ]
    const mockStore = {
      students: mockStudents,
      loading: false,
      fetchStudents: vi.fn()
    }
    vi.mocked(useStudentStore).mockReturnValue(mockStore as any)

    // Act
    render(StudentList)

    // Assert
    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument()
    })
  })

  it('displays error message when fetch fails', async () => {
    // Arrange
    const mockStore = {
      students: [],
      loading: false,
      error: new Error('Failed to fetch'),
      fetchStudents: vi.fn()
    }
    vi.mocked(useStudentStore).mockReturnValue(mockStore as any)

    // Act
    render(StudentList)

    // Assert
    expect(screen.getByText(/failed to fetch/i)).toBeInTheDocument()
  })
})
```

### Setup Files

```typescript
// vitest.config.ts
import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath } from 'node:url'

export default defineConfig({
  plugins: [vue()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/frontend/test/setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      exclude: [
        'node_modules/',
        'src/frontend/test/',
        '**/*.spec.ts',
        '**/*.test.ts'
      ]
    }
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src/frontend', import.meta.url))
    }
  }
})

// src/frontend/test/setup.ts
import { expect, afterEach } from 'vitest'
import { cleanup } from '@testing-library/vue'
import matchers from '@testing-library/jest-dom/matchers'

expect.extend(matchers)

afterEach(() => {
  cleanup()
})
```

## Matchers and Assertions

```typescript
// Basic
expect(value).toBe(expected)
expect(value).toEqual(expected) // Deep equality
expect(value).not.toBe(expected)

// Truthiness
expect(value).toBeTruthy()
expect(value).toBeFalsy()
expect(value).toBeNull()
expect(value).toBeUndefined()
expect(value).toBeDefined()

// Numbers
expect(value).toBeGreaterThan(3)
expect(value).toBeLessThanOrEqual(5)
expect(value).toBeCloseTo(0.3) // Floating point

// Strings
expect(string).toMatch(/pattern/)
expect(string).toContain('substring')

// Arrays/Iterables
expect(array).toContain(item)
expect(array).toHaveLength(3)

// Objects
expect(object).toHaveProperty('key')
expect(object).toMatchObject({ key: 'value' })

// Functions
expect(fn).toHaveBeenCalled()
expect(fn).toHaveBeenCalledTimes(2)
expect(fn).toHaveBeenCalledWith(arg1, arg2)

// Testing Library
expect(element).toBeInTheDocument()
expect(element).toHaveClass('active')
expect(element).toHaveTextContent('text')
expect(input).toHaveValue('value')
```

## Validation Checklist

- [ ] Tests follow Arrange-Act-Assert pattern
- [ ] Test names describe behavior being tested
- [ ] Mocks cleared between tests (`beforeEach`)
- [ ] No hardcoded delays (`setTimeout`)
- [ ] Async tests use `async/await` or return promises
- [ ] Components tested with Testing Library patterns
- [ ] Stores tested in isolation with Pinia test utils
- [ ] API calls mocked consistently
- [ ] Coverage above 80% threshold
- [ ] Fast execution (<1s for unit tests)

## Anti-Patterns

❌ Testing implementation details
✅ Test user-facing behavior

❌ Not cleaning up mocks between tests
✅ Use `beforeEach(() => vi.clearAllMocks())`

❌ Using `setTimeout` for async
✅ Use `await waitFor()` or proper promises

❌ Shallow rendering when not needed
✅ Full render with Testing Library

❌ Testing library internals
✅ Test public API and user interactions

❌ Hardcoded test data reused everywhere
✅ Test data factories or builders

❌ Generic test names (`it('works')`)
✅ Descriptive names (`it('displays error when fetch fails')`)

❌ Multiple unrelated assertions
✅ Focused tests with single concern
