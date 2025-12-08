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

## Next Steps

1. Add shadcn/ui component library
2. Implement routing with React Router
3. Add state management (Context API or Zustand)
4. Create customer, job, and financial UI components
5. Implement authentication with Auth0

See [business-management-app-analysis.md](../business-management-app-analysis.md) for full requirements.
