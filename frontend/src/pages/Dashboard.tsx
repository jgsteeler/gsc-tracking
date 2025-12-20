import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Users, Briefcase, DollarSign, TrendingUp } from 'lucide-react'
import { useDashboard } from '@/hooks/useDashboard'
import { JOB_STATUS_LABELS, JOB_STATUS_COLORS } from '@/types/job'

export default function Dashboard() {
  const { stats, loading, error } = useDashboard()

  const statsCards = [
    {
      name: 'Total Customers',
      value: stats.totalCustomers.toString(),
      icon: Users,
      description: 'Total registered customers',
    },
    {
      name: 'Active Jobs',
      value: stats.activeJobs.toString(),
      icon: Briefcase,
      description: 'Jobs currently in progress',
    },
    {
      name: 'Revenue (MTD)',
      value: `$${stats.revenue.toFixed(2)}`,
      icon: DollarSign,
      description: 'Revenue this month',
    },
    {
      name: 'Total Jobs',
      value: stats.totalJobs.toString(),
      icon: TrendingUp,
      description: 'Total jobs tracked',
    },
  ]

  if (error) {
    return (
      <div className="space-y-6">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Dashboard</h2>
          <p className="text-muted-foreground">
            Welcome to GSC Small Engine Repair management system
          </p>
        </div>
        <Card>
          <CardContent className="pt-6">
            <p className="text-destructive">Error loading dashboard: {error}</p>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold tracking-tight">Dashboard</h2>
        <p className="text-muted-foreground">
          Welcome to GSC Small Engine Repair management system
        </p>
      </div>

      {/* Stats Grid */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {statsCards.map((stat) => {
          const Icon = stat.icon
          return (
            <Card key={stat.name}>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">
                  {stat.name}
                </CardTitle>
                <Icon className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                {loading ? (
                  <div className="space-y-2">
                    <div className="h-8 w-20 bg-muted animate-pulse rounded" />
                    <div className="h-4 w-full bg-muted animate-pulse rounded" />
                  </div>
                ) : (
                  <>
                    <div className="text-2xl font-bold">{stat.value}</div>
                    <p className="text-xs text-muted-foreground">
                      {stat.description}
                    </p>
                  </>
                )}
              </CardContent>
            </Card>
          )
        })}
      </div>

      {/* Recent Activity */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
        <Card className="col-span-4">
          <CardHeader>
            <CardTitle>Recent Jobs</CardTitle>
            <CardDescription>
              Latest job activities in your business
            </CardDescription>
          </CardHeader>
          <CardContent>
            {loading ? (
              <div className="space-y-4">
                <div className="h-12 w-full bg-muted animate-pulse rounded" />
                <div className="h-12 w-full bg-muted animate-pulse rounded" />
                <div className="h-12 w-full bg-muted animate-pulse rounded" />
              </div>
            ) : stats.recentJobs.length === 0 ? (
              <p className="text-muted-foreground text-sm">No jobs yet. Create your first job to get started!</p>
            ) : (
              <div className="space-y-4">
                {stats.recentJobs.map((job) => (
                  <div key={job.id} className="flex items-center justify-between border-b pb-4 last:border-0 last:pb-0">
                    <div className="space-y-1">
                      <p className="text-sm font-medium leading-none">{job.customerName}</p>
                      <p className="text-sm text-muted-foreground">
                        {job.equipmentType} - {job.equipmentModel}
                      </p>
                    </div>
                    <Badge className={JOB_STATUS_COLORS[job.status]}>
                      {JOB_STATUS_LABELS[job.status]}
                    </Badge>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
        <Card className="col-span-3">
          <CardHeader>
            <CardTitle>Recent Customers</CardTitle>
            <CardDescription>
              Newly added customers
            </CardDescription>
          </CardHeader>
          <CardContent>
            {loading ? (
              <div className="space-y-4">
                <div className="h-12 w-full bg-muted animate-pulse rounded" />
                <div className="h-12 w-full bg-muted animate-pulse rounded" />
                <div className="h-12 w-full bg-muted animate-pulse rounded" />
              </div>
            ) : stats.recentCustomers.length === 0 ? (
              <p className="text-muted-foreground text-sm">No customers yet. Add your first customer to get started!</p>
            ) : (
              <div className="space-y-4">
                {stats.recentCustomers.map((customer) => (
                  <div key={customer.id} className="border-b pb-4 last:border-0 last:pb-0">
                    <p className="text-sm font-medium leading-none">{customer.name}</p>
                    {customer.phone && (
                      <p className="text-sm text-muted-foreground mt-1">{customer.phone}</p>
                    )}
                    {customer.email && (
                      <p className="text-sm text-muted-foreground">{customer.email}</p>
                    )}
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
