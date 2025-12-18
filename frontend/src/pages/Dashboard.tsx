import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import { Button } from '@/components/ui/button'
import { useToast } from '@/hooks/use-toast'
import { Users, Briefcase, DollarSign, TrendingUp } from 'lucide-react'

export default function Dashboard() {
  const { toast } = useToast()
  // Mock data - will be replaced with real API calls
  const stats = [
    {
      name: 'Total Customers',
      value: '0',
      icon: Users,
      change: '+0%',
      changeType: 'positive',
    },
    {
      name: 'Active Jobs',
      value: '0',
      icon: Briefcase,
      change: '+0%',
      changeType: 'positive',
    },
    {
      name: 'Revenue (MTD)',
      value: '$0',
      icon: DollarSign,
      change: '+0%',
      changeType: 'positive',
    },
    {
      name: 'Growth',
      value: '0%',
      icon: TrendingUp,
      change: '+0%',
      changeType: 'positive',
    },
  ]

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Dashboard</h2>
          <p className="text-muted-foreground">
            Welcome to GSC Small Engine Repair management system
          </p>
        </div>
        <Button
          variant="outline"
          onClick={() => {
            toast({
              title: "Welcome!",
              description: "Toast notifications are now working.",
            })
          }}
        >
          Test Toast
        </Button>
      </div>

      {/* Stats Grid */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {stats.map((stat) => {
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
                <div className="text-2xl font-bold">{stat.value}</div>
                <p className="text-xs text-muted-foreground">
                  {stat.change} from last month
                </p>
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
            <div className="space-y-4">
              <Skeleton className="h-12 w-full" />
              <Skeleton className="h-12 w-full" />
              <Skeleton className="h-12 w-full" />
            </div>
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
            <div className="space-y-4">
              <Skeleton className="h-12 w-full" />
              <Skeleton className="h-12 w-full" />
              <Skeleton className="h-12 w-full" />
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
