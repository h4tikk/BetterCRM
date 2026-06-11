import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { getToken, http, setToken } from '@/api/http'

export interface AuthUser {
  userId: string
  fullName: string
  role: string
  organizationId: string
  departmentId: string | null
  isMainDirector: boolean
}

interface AuthResponse extends AuthUser {
  token: string
}

interface MeResponse {
  id: string
  email: string
  fullName: string
  role: string
  organizationId: string
  departmentId: string | null
  isMainDirector: boolean
}

export interface RegisterPayload {
  organizationName: string
  fullName: string
  email: string
  password: string
  positionTitle: string
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AuthUser | null>(null)
  const token = ref<string | null>(getToken())

  const isAuthenticated = computed(() => token.value !== null)

  function applyAuth(res: AuthResponse) {
    token.value = res.token
    setToken(res.token)
    user.value = {
      userId: res.userId,
      fullName: res.fullName,
      role: res.role,
      organizationId: res.organizationId,
      departmentId: res.departmentId ?? null,
      isMainDirector: res.isMainDirector,
    }
  }

  async function login(email: string, password: string) {
    applyAuth(await http.post<AuthResponse>('/api/auth/login', { email, password }))
  }

  async function register(payload: RegisterPayload) {
    applyAuth(await http.post<AuthResponse>('/api/auth/register', payload))
  }

  async function fetchMe() {
    if (!token.value) return
    try {
      const me = await http.get<MeResponse>('/api/auth/me')
      user.value = {
        userId: me.id,
        fullName: me.fullName,
        role: me.role,
        organizationId: me.organizationId,
        departmentId: me.departmentId ?? null,
        isMainDirector: me.isMainDirector,
      }
    } catch {
      logout()
    }
  }

  function logout() {
    token.value = null
    user.value = null
    setToken(null)
  }

  return { user, token, isAuthenticated, login, register, fetchMe, logout }
})
