import { HttpClient } from './httpClient'
import { CORE_BASE_URL } from './config'

export const coreClient = new HttpClient(CORE_BASE_URL)
