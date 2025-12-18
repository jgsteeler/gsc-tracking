# GSC Tracking - Frontend

React + TypeScript + Vite frontend for the GSC Small Engine Repair business management application.

## Prerequisites

- [Node.js](https://nodejs.org/) (v18 or higher)
- [npm](https://www.npmjs.com/) (comes with Node.js)

## Getting Started

### Install Dependencies

```bash
cd frontend
npm install
```

### Running the Development Server

```bash
npm run dev
```

The app will start at `http://localhost:5173` (Vite's default port).

### Building for Production

```bash
npm run build
```

This will create an optimized production build in the `dist/` directory.

### Preview Production Build

```bash
npm run preview
```

## Running Tests

### Run all tests
```bash
npm run test
```

### Run tests with UI
```bash
npm run test:ui
```

### Run tests with coverage
```bash
npm run test:coverage
```

The coverage report will be generated in the `coverage/` directory.

### Current Coverage
- **Overall**: 96.61% line coverage
- **Utils**: 100% coverage
- **Services**: 92.85% coverage (customerService)
- **Hooks**: 100% coverage (useCustomers)

## Features

This is a Hello World setup that:
- Connects to the backend API at `https://localhost:5001`
- Displays API status and messages
- Uses React with TypeScript for type safety
- Uses Vite for fast development and building

## Project Structure

```
frontend/
├── src/
│   ├── App.tsx          # Main application component
│   ├── App.css          # Application styles
│   ├── main.tsx         # Application entry point
│   └── index.css        # Global styles
├── public/              # Static assets
├── index.html           # HTML template
└── vite.config.ts       # Vite configuration
```

## Features

### Implemented ✅
- **shadcn/ui Component Library**: Tailwind CSS-based UI components
- **React Router**: Navigation between Dashboard, Customers, and Jobs pages
- **Responsive Layout**: Desktop sidebar and mobile hamburger menu
- **shadcn/ui Components**: Button, Card, Skeleton, Toast, Badge, Input, Dialog, Label, Tabs
- **Toast Notifications**: User feedback system using Radix UI
- **Loading States**: Skeleton components for async operations
- **Modern Styling**: Tailwind CSS with CSS variables for theming
- **Auth0 Integration**: Authentication system with login/logout functionality
- **Auth0 Components**: LoginButton, LogoutButton, UserProfile, Auth0ProviderWithNavigate, ProtectedRoute (preparatory)

### Authentication Components

The application includes Auth0 authentication components:

- `LoginButton` - Triggers Auth0 login flow
- `LogoutButton` - Logs user out and clears session
- `UserProfile` - Displays user avatar and information
- `Auth0ProviderWithNavigate` - Auth0 provider with React Router integration
- `ProtectedRoute` - Route protection component (**not currently integrated**)

**Note on ProtectedRoute**: This component is included as preparatory work for future use when route-level authentication becomes required. It is not currently integrated into the routing but can be easily added when needed. See `docs/AUTH0-SETUP.md` for usage examples.

The application currently uses optional authentication - users can browse the app without logging in, with certain features restricted based on authentication status.

### Component Library

The following shadcn/ui components are available:
- `Button` - Various button variants and sizes
- `Card` - Content containers with header, content, and footer
- `Skeleton` - Loading placeholders
- `Toast` - Notification system with useToast hook
- `Badge` - Status indicators
- `Input` - Form input fields
- `Dialog` - Modal dialogs
- `Label` - Form labels
- `Tabs` - Tabbed interfaces

### Pages

- **Dashboard**: Overview with stats cards and recent activity
- **Customers**: Customer list with skeleton loading states
- **Jobs**: Job tracking interface with skeleton loading states

## Next Steps

1. ✅ Add shadcn/ui component library
2. ✅ Implement routing with React Router
3. ✅ Create responsive navigation layout
4. ✅ Implement authentication with Auth0
5. Add state management (TanStack Query or Zustand)
6. Connect to backend API for real data
7. Add Table, Select, DropdownMenu components as needed
8. Implement dark mode toggle (optional)
9. Integrate ProtectedRoute for route-level authentication (when required)

See [business-management-app-analysis.md](../../docs/business-management-app-analysis.md) for full requirements.
