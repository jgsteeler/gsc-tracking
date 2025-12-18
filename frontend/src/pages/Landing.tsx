import { useAuth0 } from '@auth0/auth0-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Wrench, Users, Briefcase, TrendingUp, Shield, Clock } from 'lucide-react';

/**
 * Landing page shown to unauthenticated users
 * Provides information about the application and a login button
 */
export default function Landing() {
  const { loginWithRedirect, isLoading } = useAuth0();

  const features = [
    {
      icon: Users,
      title: 'Customer Management',
      description: 'Track and manage all your customer information in one place',
    },
    {
      icon: Briefcase,
      title: 'Job Tracking',
      description: 'Monitor repairs, maintenance, and service jobs from start to finish',
    },
    {
      icon: TrendingUp,
      title: 'Business Insights',
      description: 'Get real-time analytics and reports on your business performance',
    },
    {
      icon: Shield,
      title: 'Secure & Reliable',
      description: 'Your data is protected with enterprise-grade security',
    },
    {
      icon: Clock,
      title: 'Save Time',
      description: 'Streamline your workflow and focus on what matters most',
    },
    {
      icon: Wrench,
      title: 'Small Engine Experts',
      description: 'Built specifically for small engine repair businesses',
    },
  ];

  return (
    <div className="min-h-screen bg-gradient-to-b from-background to-muted">
      {/* Hero Section */}
      <div className="container mx-auto px-4 py-16">
        <div className="text-center max-w-3xl mx-auto mb-16">
          <div className="flex items-center justify-center mb-6">
            <Wrench className="h-16 w-16 text-primary" />
          </div>
          <h1 className="text-4xl md:text-6xl font-bold mb-6 tracking-tight">
            GSC Tracking
          </h1>
          <p className="text-xl md:text-2xl text-muted-foreground mb-12">
            Complete business management solution for small engine repair shops
          </p>
          <Button
            size="lg"
            onClick={() => loginWithRedirect()}
            disabled={isLoading}
            className="text-xl px-12 py-8 font-semibold shadow-2xl hover:shadow-xl transition-all duration-300 bg-primary hover:bg-primary/90 text-primary-foreground motion-safe:animate-pulse hover:animate-none"
            aria-label="Log in to get started with secure authentication"
          >
            <Shield className="h-6 w-6 mr-2" aria-hidden="true" />
            {isLoading ? 'Loading...' : 'Log In to Get Started'}
          </Button>
          <p className="text-sm text-muted-foreground mt-6">
            Click the button above to securely log in with Auth0
          </p>
        </div>

        {/* Features Grid */}
        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3 max-w-6xl mx-auto">
          {features.map((feature) => {
            const Icon = feature.icon;
            return (
              <Card key={feature.title} className="hover:shadow-lg transition-shadow">
                <CardHeader>
                  <div className="flex items-center space-x-3">
                    <div className="p-2 bg-primary/10 rounded-lg">
                      <Icon className="h-6 w-6 text-primary" />
                    </div>
                    <CardTitle className="text-lg">{feature.title}</CardTitle>
                  </div>
                </CardHeader>
                <CardContent>
                  <CardDescription className="text-base">
                    {feature.description}
                  </CardDescription>
                </CardContent>
              </Card>
            );
          })}
        </div>

        {/* Footer Info */}
        <div className="text-center mt-16 pt-16 border-t">
          <p className="text-muted-foreground mb-4">
            Trusted by Gibson Service Company, LLC
          </p>
          <a
            href="https://gibsonservice.co"
            target="_blank"
            rel="noopener noreferrer"
            className="text-primary hover:underline"
          >
            Learn More About GSC
          </a>
        </div>
      </div>
    </div>
  );
}
