import { HttpClient } from './httpClient'
import { IDENTITY_BASE_URL } from './config'

export const identityClient = new HttpClient(IDENTITY_BASE_URL)
