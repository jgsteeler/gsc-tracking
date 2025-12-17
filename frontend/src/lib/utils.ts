import { type ClassValue, clsx } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

/**
 * Formats a date string for API submission
 * Ensures the date is in ISO 8601 format with time component
 * @param date - Date string in YYYY-MM-DD or ISO format
 * @returns Date string in ISO 8601 format (YYYY-MM-DDTHH:mm:ssZ)
 */
export function formatDateForApi(date: string): string {
  return date.includes('T') ? date : `${date}T00:00:00Z`
}
