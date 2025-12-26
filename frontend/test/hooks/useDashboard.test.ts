import { describe, it, expect, beforeEach, vi } from 'vitest';
import { renderHook, waitFor } from '@testing-library/react';
import { useDashboard } from '../../src/hooks/useDashboard';
import { customerService } from '@/services/customerService';
import { jobService } from '@/services/jobService';
import type { Customer } from '@/types/customer';
import type { Job } from '@/types/job';

// Mock the services
vi.mock('@/services/customerService', () => ({
  customerService: {
    getAll: vi.fn(),
  },
}));

vi.mock('@/services/jobService', () => ({
  jobService: {
    getAll: vi.fn(),
  },
}));

// Mock useAccessToken with a stable getToken function
const mockGetToken = vi.fn().mockResolvedValue(null);
vi.mock('@/hooks/useAccessToken', () => ({
  useAccessToken: () => ({
    getToken: mockGetToken,
    isAuthEnabled: false,
    isAuthenticated: false,
  }),
}));

describe('useDashboard', () => {
  const now = new Date();
  const currentMonth = now.getMonth();
  const currentYear = now.getFullYear();
  const thisMonthDate = new Date(currentYear, currentMonth, 15).toISOString();
  const lastMonthDate = new Date(currentYear, currentMonth - 1, 15).toISOString();

  const mockCustomers: Customer[] = [
    {
      id: 1,
      name: 'John Doe',
      email: 'john@example.com',
      phone: '1234567890',
      address: '123 Main St',
      createdAt: thisMonthDate,
      updatedAt: thisMonthDate,
    },
    {
      id: 2,
      name: 'Jane Smith',
      email: 'jane@example.com',
      phone: '0987654321',
      address: '456 Oak Ave',
      createdAt: lastMonthDate,
      updatedAt: lastMonthDate,
    },
    {
      id: 3,
      name: 'Bob Johnson',
      email: 'bob@example.com',
      phone: '5555555555',
      address: '789 Pine Rd',
      createdAt: lastMonthDate,
      updatedAt: lastMonthDate,
    },
  ];

  const mockJobs: Job[] = [
    {
      id: 1,
      customerId: 1,
      customerName: 'John Doe',
      equipmentType: 'Lawn Mower',
      equipmentModel: 'XYZ-100',
      description: 'Blade sharpening',
      status: 'InProgress',
      dateReceived: thisMonthDate,
      dateCompleted: null,
      estimateAmount: 50.00,
      actualAmount: null,
      totalCost: 50.00,
      profitMargin: null,
      createdAt: thisMonthDate,
      updatedAt: thisMonthDate,
    },
    {
      id: 2,
      customerId: 2,
      customerName: 'Jane Smith',
      equipmentType: 'Chainsaw',
      equipmentModel: 'CS-200',
      description: 'Chain replacement',
      status: 'Completed',
      dateReceived: thisMonthDate,
      dateCompleted: thisMonthDate,
      estimateAmount: 100.00,
      actualAmount: 95.00,
      totalCost: 95.00,
      profitMargin: 20.00,
      createdAt: thisMonthDate,
      updatedAt: thisMonthDate,
    },
    {
      id: 3,
      customerId: 3,
      customerName: 'Bob Johnson',
      equipmentType: 'Weed Trimmer',
      equipmentModel: 'WT-50',
      description: 'String replacement',
      status: 'Paid',
      dateReceived: lastMonthDate,
      dateCompleted: lastMonthDate,
      estimateAmount: 30.00,
      actualAmount: 30.00,
      totalCost: 30.00,
      profitMargin: 10.00,
      createdAt: lastMonthDate,
      updatedAt: lastMonthDate,
    },
    {
      id: 4,
      customerId: 1,
      customerName: 'John Doe',
      equipmentType: 'Blower',
      equipmentModel: 'BL-300',
      description: 'Engine tune-up',
      status: 'InProgress',
      dateReceived: thisMonthDate,
      dateCompleted: null,
      estimateAmount: 75.00,
      actualAmount: null,
      totalCost: 75.00,
      profitMargin: null,
      createdAt: thisMonthDate,
      updatedAt: thisMonthDate,
    },
    {
      id: 5,
      customerId: 2,
      customerName: 'Jane Smith',
      equipmentType: 'Tiller',
      equipmentModel: 'TL-400',
      description: 'Blade repair',
      status: 'Quote',
      dateReceived: thisMonthDate,
      dateCompleted: null,
      estimateAmount: 120.00,
      actualAmount: null,
      totalCost: 120.00,
      profitMargin: null,
      createdAt: thisMonthDate,
      updatedAt: thisMonthDate,
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should fetch and calculate dashboard statistics', async () => {
    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs);

    const { result } = renderHook(() => useDashboard());

    expect(result.current.loading).toBe(true);
    expect(result.current.stats).toEqual({
      totalCustomers: 0,
      totalJobs: 0,
      activeJobs: 0,
      revenue: 0,
      recentJobs: [],
      recentCustomers: [],
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.stats.totalCustomers).toBe(3);
    expect(result.current.stats.totalJobs).toBe(5);
    expect(result.current.stats.activeJobs).toBe(2); // Two jobs with InProgress status
    expect(result.current.stats.revenue).toBe(95.00); // Only job 2 is completed this month
    expect(result.current.stats.recentJobs.length).toBe(5);
    expect(result.current.stats.recentCustomers.length).toBe(3);
    expect(result.current.error).toBeNull();
  });

  it('should sort recent jobs by creation date (newest first)', async () => {
    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    const recentJobs = result.current.stats.recentJobs;
    expect(recentJobs.length).toBe(5);
    // Jobs created this month should be first
    expect(recentJobs[0].id).toBe(1);
    expect(recentJobs[1].id).toBe(2);
  });

  it('should sort recent customers by creation date (newest first)', async () => {
    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    const recentCustomers = result.current.stats.recentCustomers;
    expect(recentCustomers.length).toBe(3);
    // Customer created this month should be first
    expect(recentCustomers[0].id).toBe(1);
  });

  it('should limit recent jobs to 5 items', async () => {
    const manyJobs = Array.from({ length: 10 }, (_, i) => ({
      ...mockJobs[0],
      id: i + 1,
      createdAt: thisMonthDate,
    }));

    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(jobService.getAll).mockResolvedValueOnce(manyJobs);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.stats.recentJobs.length).toBe(5);
  });

  it('should limit recent customers to 5 items', async () => {
    const manyCustomers = Array.from({ length: 10 }, (_, i) => ({
      ...mockCustomers[0],
      id: i + 1,
      createdAt: thisMonthDate,
    }));

    vi.mocked(customerService.getAll).mockResolvedValueOnce(manyCustomers);
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.stats.recentCustomers.length).toBe(5);
  });

  it('should handle empty data', async () => {
    vi.mocked(customerService.getAll).mockResolvedValueOnce([]);
    vi.mocked(jobService.getAll).mockResolvedValueOnce([]);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.stats.totalCustomers).toBe(0);
    expect(result.current.stats.totalJobs).toBe(0);
    expect(result.current.stats.activeJobs).toBe(0);
    expect(result.current.stats.revenue).toBe(0);
    expect(result.current.stats.recentJobs).toEqual([]);
    expect(result.current.stats.recentCustomers).toEqual([]);
    expect(result.current.error).toBeNull();
  });

  it('should handle fetch error from customer service', async () => {
    const errorMessage = 'Failed to fetch customers';
    vi.mocked(customerService.getAll).mockRejectedValueOnce(new Error(errorMessage));
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBe(errorMessage);
    expect(result.current.stats).toEqual({
      totalCustomers: 0,
      totalJobs: 0,
      activeJobs: 0,
      revenue: 0,
      recentJobs: [],
      recentCustomers: [],
    });
  });

  it('should handle fetch error from job service', async () => {
    const errorMessage = 'Failed to fetch jobs';
    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(jobService.getAll).mockRejectedValueOnce(new Error(errorMessage));

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBe(errorMessage);
    expect(result.current.stats).toEqual({
      totalCustomers: 0,
      totalJobs: 0,
      activeJobs: 0,
      revenue: 0,
      recentJobs: [],
      recentCustomers: [],
    });
  });

  it('should calculate revenue only from current month', async () => {
    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    // Only job 2 (Completed, $95 this month) should count
    // Job 3 (Paid, $30) is from last month, so it shouldn't count
    expect(result.current.stats.revenue).toBe(95.00);
  });

  it('should use actualAmount over estimateAmount for revenue', async () => {
    const jobsWithEstimates: Job[] = [
      {
        ...mockJobs[0],
        status: 'Completed',
        dateCompleted: thisMonthDate,
        estimateAmount: 100.00,
        actualAmount: 80.00, // Should use this
      },
    ];

    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(jobService.getAll).mockResolvedValueOnce(jobsWithEstimates);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.stats.revenue).toBe(80.00);
  });

  it('should use estimateAmount if actualAmount is not available', async () => {
    const jobsWithEstimatesOnly: Job[] = [
      {
        ...mockJobs[0],
        status: 'Completed',
        dateCompleted: thisMonthDate,
        estimateAmount: 100.00,
        actualAmount: null, // No actual amount
      },
    ];

    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(jobService.getAll).mockResolvedValueOnce(jobsWithEstimatesOnly);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.stats.revenue).toBe(100.00);
  });

  it('should refetch dashboard data', async () => {
    vi.mocked(customerService.getAll)
      .mockResolvedValueOnce(mockCustomers)
      .mockResolvedValueOnce([mockCustomers[0]]);
    vi.mocked(jobService.getAll)
      .mockResolvedValueOnce(mockJobs)
      .mockResolvedValueOnce([mockJobs[0]]);

    const { result } = renderHook(() => useDashboard());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.stats.totalCustomers).toBe(3);
    expect(result.current.stats.recentJobs.length).toBe(5);

    await waitFor(async () => {
      await result.current.refetch();
    });

    await waitFor(() => {
      expect(result.current.stats.totalCustomers).toBe(1);
    });

    expect(result.current.stats.recentJobs.length).toBe(1);
  });
});
