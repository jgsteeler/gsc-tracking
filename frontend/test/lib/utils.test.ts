import { describe, it, expect } from 'vitest';
import { cn, formatDateForApi } from '../../src/lib/utils';

describe('cn utility function', () => {
  it('should merge class names correctly', () => {
    const result = cn('class1', 'class2');
    expect(result).toBe('class1 class2');
  });

  it('should handle conditional classes', () => {
    const isConditional = false;
    const isIncluded = true;
    const result = cn('base', isConditional && 'conditional', isIncluded && 'included');
    expect(result).toBe('base included');
  });

  it('should handle undefined and null values', () => {
    const result = cn('class1', undefined, null, 'class2');
    expect(result).toBe('class1 class2');
  });

  it('should merge tailwind classes correctly', () => {
    const result = cn('px-2 py-1', 'px-4');
    expect(result).toBe('py-1 px-4');
  });

  it('should handle empty input', () => {
    const result = cn();
    expect(result).toBe('');
  });

  it('should handle array of classes', () => {
    const result = cn(['class1', 'class2'], 'class3');
    expect(result).toBe('class1 class2 class3');
  });

  it('should handle object notation', () => {
    const result = cn({
      'class1': true,
      'class2': false,
      'class3': true,
    });
    expect(result).toBe('class1 class3');
  });
});

describe('formatDateForApi utility function', () => {
  it('should add time component to date-only string', () => {
    const result = formatDateForApi('2024-01-15');
    expect(result).toBe('2024-01-15T00:00:00Z');
  });

  it('should return ISO format string unchanged', () => {
    const isoDate = '2024-01-15T14:30:00Z';
    const result = formatDateForApi(isoDate);
    expect(result).toBe(isoDate);
  });

  it('should handle ISO format with milliseconds', () => {
    const isoDate = '2024-01-15T14:30:00.123Z';
    const result = formatDateForApi(isoDate);
    expect(result).toBe(isoDate);
  });

  it('should handle ISO format with timezone offset', () => {
    const isoDate = '2024-01-15T14:30:00+05:00';
    const result = formatDateForApi(isoDate);
    expect(result).toBe(isoDate);
  });
});
