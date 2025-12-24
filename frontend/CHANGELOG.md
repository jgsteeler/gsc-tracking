# Changelog

## [2.5.0](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v2.4.2...frontend-v2.5.0) (2025-12-24)


### Features

* **auth:** implement three-tier role-based access control with Auth0 permissions support ([#181](https://github.com/jgsteeler/gsc-tracking/issues/181)) ([6ba3bbf](https://github.com/jgsteeler/gsc-tracking/commit/6ba3bbf4cce7a1cb0272dbeed830d16305932b9d))

## [2.4.2](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v2.4.1...frontend-v2.4.2) (2025-12-24)


### Bug Fixes

* **auth:** memoize getToken to prevent infinite loop ([#175](https://github.com/jgsteeler/gsc-tracking/issues/175)) ([74b405e](https://github.com/jgsteeler/gsc-tracking/commit/74b405e26e63ff3e9a57fd8b42d311471ad599de))

## [2.4.1](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v2.4.0...frontend-v2.4.1) (2025-12-23)


### Bug Fixes

* **auth:** pass access tokens to API endpoints ([#173](https://github.com/jgsteeler/gsc-tracking/issues/173)) ([dffb426](https://github.com/jgsteeler/gsc-tracking/commit/dffb426fcb3030d985a59ff57af979041fd37d2d))

## [2.4.0](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v2.3.0...frontend-v2.4.0) (2025-12-21)


### Features

* **mvp:** implement CSV import/export for expenses and jobs ([#171](https://github.com/jgsteeler/gsc-tracking/issues/171)) ([08cfd4f](https://github.com/jgsteeler/gsc-tracking/commit/08cfd4f938863788a02a3f7b9a42529c18aae3e1))

## [2.3.0](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v2.2.0...frontend-v2.3.0) (2025-12-21)


### Features

* **auth:** implement UI authorization based on tracker-admin role ([#169](https://github.com/jgsteeler/gsc-tracking/issues/169)) ([6000c78](https://github.com/jgsteeler/gsc-tracking/commit/6000c78ed6c4b0df1a2ef4d7732936ebd254737b))

## [2.2.0](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v2.1.0...frontend-v2.2.0) (2025-12-21)


### Features

* **ui:** replace Vite favicon with custom GSC-themed favicon ([#164](https://github.com/jgsteeler/gsc-tracking/issues/164)) ([099bc9a](https://github.com/jgsteeler/gsc-tracking/commit/099bc9a28ea3f8ba1136d836da501788483feb48))

## [2.1.0](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v2.0.0...frontend-v2.1.0) (2025-12-20)


### Features

* **dashboard:** implement functional dashboard with real API data ([#158](https://github.com/jgsteeler/gsc-tracking/issues/158)) ([e7770cf](https://github.com/jgsteeler/gsc-tracking/commit/e7770cf83737ec65d694a143f4ab1cdb6f3ff783))


### Bug Fixes

* **docs:** update Auth0 setup instructions for local development ([#161](https://github.com/jgsteeler/gsc-tracking/issues/161)) ([d59003e](https://github.com/jgsteeler/gsc-tracking/commit/d59003eca4c77e4c8ea65309938d146885881a01))

## [2.0.0](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v1.0.0...frontend-v2.0.0) (2025-12-20)


### ⚠ BREAKING CHANGES

* Application has been deployed to production behind authentication. Marking this milestone as version 1.0.0 for both frontend and backend components.

### Features

* **auth:** add landing page with prominent accessible login ([#147](https://github.com/jgsteeler/gsc-tracking/issues/147)) ([ff5c6af](https://github.com/jgsteeler/gsc-tracking/commit/ff5c6af5085a7870daedc800a5910c08ea007271))
* **auth:** implement Auth0 authentication for backend and frontend  ([#129](https://github.com/jgsteeler/gsc-tracking/issues/129)) ([472e2f4](https://github.com/jgsteeler/gsc-tracking/commit/472e2f46c16a1f7864d5edf2cec6615ad01bb56f))
* **customer:** implement CRUD operations with search and soft delete ([#48](https://github.com/jgsteeler/gsc-tracking/issues/48)) ([c23ae95](https://github.com/jgsteeler/gsc-tracking/commit/c23ae952920f918b3773ba8ab5fa1adf62f3281d))
* **customer:** implement customer management with CRUD operations ([#83](https://github.com/jgsteeler/gsc-tracking/issues/83)) ([217b831](https://github.com/jgsteeler/gsc-tracking/commit/217b831dd32e21ac0260cddbe6f4d0267c810c44))
* **dashboard:** implement functional dashboard with real API data ([#158](https://github.com/jgsteeler/gsc-tracking/issues/158)) ([e7770cf](https://github.com/jgsteeler/gsc-tracking/commit/e7770cf83737ec65d694a143f4ab1cdb6f3ff783))
* **docs:** add setup instructions and business management app analysis ([#140](https://github.com/jgsteeler/gsc-tracking/issues/140)) ([de7eb3a](https://github.com/jgsteeler/gsc-tracking/commit/de7eb3ae8424c374de59d7968a42916800b03c5b))
* **expense:** add backend expense tracking system with CRUD operations ([#121](https://github.com/jgsteeler/gsc-tracking/issues/121)) ([0fa2ec8](https://github.com/jgsteeler/gsc-tracking/commit/0fa2ec8c94222b5f1aa8af1cf32495c8c63662e8))
* **infra:** add Netlify deployment with deploy previews as staging ([#55](https://github.com/jgsteeler/gsc-tracking/issues/55)) ([8795b4c](https://github.com/jgsteeler/gsc-tracking/commit/8795b4c194fbd99acd4fb92d2104038dc57b10eb))
* **job:** add customer creation from job screen ([#88](https://github.com/jgsteeler/gsc-tracking/issues/88)) ([4fb3086](https://github.com/jgsteeler/gsc-tracking/commit/4fb308601cadf9ed74e416a43341736998e4b93e))
* **job:** add job updates for tracking progress notes ([#96](https://github.com/jgsteeler/gsc-tracking/issues/96)) ([897adbf](https://github.com/jgsteeler/gsc-tracking/commit/897adbf40bd3413e7b5af85eb33465ab9d94f5f2))
* **job:** implement job management with CRUD operations and unit tests ([#68](https://github.com/jgsteeler/gsc-tracking/issues/68)) ([e0b98be](https://github.com/jgsteeler/gsc-tracking/commit/e0b98be69902e7c5889bb8f6fc8219e239b1f1ff))
* **test:** implement unit testing framework with CI/CD integration ([#59](https://github.com/jgsteeler/gsc-tracking/issues/59)) ([584d3a8](https://github.com/jgsteeler/gsc-tracking/commit/584d3a809e14466c236a3ec7afa53812ac9e750f))
* **validation:** implement comprehensive form validation with Zod and FluentValidation ([#101](https://github.com/jgsteeler/gsc-tracking/issues/101)) ([bf61ce9](https://github.com/jgsteeler/gsc-tracking/commit/bf61ce9baeb7955fe551b1f0260a32aa484bc08f))


### Bug Fixes

* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([eb690d2](https://github.com/jgsteeler/gsc-tracking/commit/eb690d2e97db09ad1587943271d1c11e28c01f82))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([fad2c07](https://github.com/jgsteeler/gsc-tracking/commit/fad2c07e181c1e4fda0d589ebc7c3afb473e8398))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([1f715b4](https://github.com/jgsteeler/gsc-tracking/commit/1f715b47b80834a728eb31aeed8acd6b35b980ae))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([71c3753](https://github.com/jgsteeler/gsc-tracking/commit/71c37535c2ab2c4f424cfda8b35468473b772ec8))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([ab1e9e2](https://github.com/jgsteeler/gsc-tracking/commit/ab1e9e274ed70e569aadc057ef35ba4b7be8f367))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([97b47ac](https://github.com/jgsteeler/gsc-tracking/commit/97b47ac5f1289a8c1958f7bba853669a31e57db7))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([e389db1](https://github.com/jgsteeler/gsc-tracking/commit/e389db1544d3cc10a39acfbef7bec959621199f4))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([0ac74a3](https://github.com/jgsteeler/gsc-tracking/commit/0ac74a33f2c104d866a01e9217d8afc683592b3e))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([b77b31e](https://github.com/jgsteeler/gsc-tracking/commit/b77b31e5df94b1621c56bbcf21b20fc659fe1f5c))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([a313eb3](https://github.com/jgsteeler/gsc-tracking/commit/a313eb3521369ae509d764900497963ce1b8470b))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([22015cb](https://github.com/jgsteeler/gsc-tracking/commit/22015cb985890b6c35201c0c32d55a7806cfc2d4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([5f52d99](https://github.com/jgsteeler/gsc-tracking/commit/5f52d99e85c0ee44b4847a168e897f774b6712f6))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([d561501](https://github.com/jgsteeler/gsc-tracking/commit/d561501c5e72afb4d82370be94b7149ec0f380a8))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([6b6c89e](https://github.com/jgsteeler/gsc-tracking/commit/6b6c89eb4c77efe902b44622af8ed79d4f2502c4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([14365d6](https://github.com/jgsteeler/gsc-tracking/commit/14365d60302c6433204f3e00c4c4655405f1faa0))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([8fd0a5a](https://github.com/jgsteeler/gsc-tracking/commit/8fd0a5a1f2fd1d8244a3d9c5d90ef343daa2ee2d))
* **expense:** align receiptReference validation with form behavior ([#127](https://github.com/jgsteeler/gsc-tracking/issues/127)) ([ca2a107](https://github.com/jgsteeler/gsc-tracking/commit/ca2a107340f4e9910dddba3b667bd3f0aeeddf38))
* **ui:** make dialog forms and toast notifications fully opaque and visible ([#64](https://github.com/jgsteeler/gsc-tracking/issues/64)) ([b7c62da](https://github.com/jgsteeler/gsc-tracking/commit/b7c62dad97a70b580327202a81321e95ecf13669))


### Miscellaneous Chores

* release version 1.0.0 for production deployment ([#154](https://github.com/jgsteeler/gsc-tracking/issues/154)) ([998a6f3](https://github.com/jgsteeler/gsc-tracking/commit/998a6f346104c56f1f178865f161b06793264fbc))

## [1.0.0](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.16...frontend-v1.0.0) (2025-12-19)


### ⚠ BREAKING CHANGES

* Application has been deployed to production behind authentication. Marking this milestone as version 1.0.0 for both frontend and backend components.

### Features

* **auth:** add landing page with prominent accessible login ([#147](https://github.com/jgsteeler/gsc-tracking/issues/147)) ([ff5c6af](https://github.com/jgsteeler/gsc-tracking/commit/ff5c6af5085a7870daedc800a5910c08ea007271))
* **auth:** implement Auth0 authentication for backend and frontend  ([#129](https://github.com/jgsteeler/gsc-tracking/issues/129)) ([472e2f4](https://github.com/jgsteeler/gsc-tracking/commit/472e2f46c16a1f7864d5edf2cec6615ad01bb56f))
* **customer:** implement CRUD operations with search and soft delete ([#48](https://github.com/jgsteeler/gsc-tracking/issues/48)) ([c23ae95](https://github.com/jgsteeler/gsc-tracking/commit/c23ae952920f918b3773ba8ab5fa1adf62f3281d))
* **customer:** implement customer management with CRUD operations ([#83](https://github.com/jgsteeler/gsc-tracking/issues/83)) ([217b831](https://github.com/jgsteeler/gsc-tracking/commit/217b831dd32e21ac0260cddbe6f4d0267c810c44))
* **docs:** add setup instructions and business management app analysis ([#140](https://github.com/jgsteeler/gsc-tracking/issues/140)) ([de7eb3a](https://github.com/jgsteeler/gsc-tracking/commit/de7eb3ae8424c374de59d7968a42916800b03c5b))
* **expense:** add backend expense tracking system with CRUD operations ([#121](https://github.com/jgsteeler/gsc-tracking/issues/121)) ([0fa2ec8](https://github.com/jgsteeler/gsc-tracking/commit/0fa2ec8c94222b5f1aa8af1cf32495c8c63662e8))
* **infra:** add Netlify deployment with deploy previews as staging ([#55](https://github.com/jgsteeler/gsc-tracking/issues/55)) ([8795b4c](https://github.com/jgsteeler/gsc-tracking/commit/8795b4c194fbd99acd4fb92d2104038dc57b10eb))
* **job:** add customer creation from job screen ([#88](https://github.com/jgsteeler/gsc-tracking/issues/88)) ([4fb3086](https://github.com/jgsteeler/gsc-tracking/commit/4fb308601cadf9ed74e416a43341736998e4b93e))
* **job:** add job updates for tracking progress notes ([#96](https://github.com/jgsteeler/gsc-tracking/issues/96)) ([897adbf](https://github.com/jgsteeler/gsc-tracking/commit/897adbf40bd3413e7b5af85eb33465ab9d94f5f2))
* **job:** implement job management with CRUD operations and unit tests ([#68](https://github.com/jgsteeler/gsc-tracking/issues/68)) ([e0b98be](https://github.com/jgsteeler/gsc-tracking/commit/e0b98be69902e7c5889bb8f6fc8219e239b1f1ff))
* **test:** implement unit testing framework with CI/CD integration ([#59](https://github.com/jgsteeler/gsc-tracking/issues/59)) ([584d3a8](https://github.com/jgsteeler/gsc-tracking/commit/584d3a809e14466c236a3ec7afa53812ac9e750f))
* **validation:** implement comprehensive form validation with Zod and FluentValidation ([#101](https://github.com/jgsteeler/gsc-tracking/issues/101)) ([bf61ce9](https://github.com/jgsteeler/gsc-tracking/commit/bf61ce9baeb7955fe551b1f0260a32aa484bc08f))


### Bug Fixes

* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([eb690d2](https://github.com/jgsteeler/gsc-tracking/commit/eb690d2e97db09ad1587943271d1c11e28c01f82))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([fad2c07](https://github.com/jgsteeler/gsc-tracking/commit/fad2c07e181c1e4fda0d589ebc7c3afb473e8398))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([1f715b4](https://github.com/jgsteeler/gsc-tracking/commit/1f715b47b80834a728eb31aeed8acd6b35b980ae))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([71c3753](https://github.com/jgsteeler/gsc-tracking/commit/71c37535c2ab2c4f424cfda8b35468473b772ec8))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([ab1e9e2](https://github.com/jgsteeler/gsc-tracking/commit/ab1e9e274ed70e569aadc057ef35ba4b7be8f367))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([97b47ac](https://github.com/jgsteeler/gsc-tracking/commit/97b47ac5f1289a8c1958f7bba853669a31e57db7))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([e389db1](https://github.com/jgsteeler/gsc-tracking/commit/e389db1544d3cc10a39acfbef7bec959621199f4))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([0ac74a3](https://github.com/jgsteeler/gsc-tracking/commit/0ac74a33f2c104d866a01e9217d8afc683592b3e))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([b77b31e](https://github.com/jgsteeler/gsc-tracking/commit/b77b31e5df94b1621c56bbcf21b20fc659fe1f5c))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([a313eb3](https://github.com/jgsteeler/gsc-tracking/commit/a313eb3521369ae509d764900497963ce1b8470b))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([22015cb](https://github.com/jgsteeler/gsc-tracking/commit/22015cb985890b6c35201c0c32d55a7806cfc2d4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([5f52d99](https://github.com/jgsteeler/gsc-tracking/commit/5f52d99e85c0ee44b4847a168e897f774b6712f6))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([d561501](https://github.com/jgsteeler/gsc-tracking/commit/d561501c5e72afb4d82370be94b7149ec0f380a8))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([6b6c89e](https://github.com/jgsteeler/gsc-tracking/commit/6b6c89eb4c77efe902b44622af8ed79d4f2502c4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([14365d6](https://github.com/jgsteeler/gsc-tracking/commit/14365d60302c6433204f3e00c4c4655405f1faa0))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([8fd0a5a](https://github.com/jgsteeler/gsc-tracking/commit/8fd0a5a1f2fd1d8244a3d9c5d90ef343daa2ee2d))
* **expense:** align receiptReference validation with form behavior ([#127](https://github.com/jgsteeler/gsc-tracking/issues/127)) ([ca2a107](https://github.com/jgsteeler/gsc-tracking/commit/ca2a107340f4e9910dddba3b667bd3f0aeeddf38))
* **ui:** make dialog forms and toast notifications fully opaque and visible ([#64](https://github.com/jgsteeler/gsc-tracking/issues/64)) ([b7c62da](https://github.com/jgsteeler/gsc-tracking/commit/b7c62dad97a70b580327202a81321e95ecf13669))


### Miscellaneous Chores

* release version 1.0.0 for production deployment ([#154](https://github.com/jgsteeler/gsc-tracking/issues/154)) ([998a6f3](https://github.com/jgsteeler/gsc-tracking/commit/998a6f346104c56f1f178865f161b06793264fbc))

## [0.1.16](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.15...frontend-v0.1.16) (2025-12-19)


### Features

* **auth:** add landing page with prominent accessible login ([#147](https://github.com/jgsteeler/gsc-tracking/issues/147)) ([ff5c6af](https://github.com/jgsteeler/gsc-tracking/commit/ff5c6af5085a7870daedc800a5910c08ea007271))

## [0.1.15](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.14...frontend-v0.1.15) (2025-12-18)


### Features

* **docs:** add setup instructions and business management app analysis ([#140](https://github.com/jgsteeler/gsc-tracking/issues/140)) ([de7eb3a](https://github.com/jgsteeler/gsc-tracking/commit/de7eb3ae8424c374de59d7968a42916800b03c5b))

## [0.1.14](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.13...frontend-v0.1.14) (2025-12-18)


### Features

* **auth:** implement Auth0 authentication for backend and frontend  ([#129](https://github.com/jgsteeler/gsc-tracking/issues/129)) ([472e2f4](https://github.com/jgsteeler/gsc-tracking/commit/472e2f46c16a1f7864d5edf2cec6615ad01bb56f))

## [0.1.13](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.12...frontend-v0.1.13) (2025-12-18)


### Bug Fixes

* **expense:** align receiptReference validation with form behavior ([#127](https://github.com/jgsteeler/gsc-tracking/issues/127)) ([ca2a107](https://github.com/jgsteeler/gsc-tracking/commit/ca2a107340f4e9910dddba3b667bd3f0aeeddf38))

## [0.1.12](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.11...frontend-v0.1.12) (2025-12-17)


### Features

* **expense:** add backend expense tracking system with CRUD operations ([#121](https://github.com/jgsteeler/gsc-tracking/issues/121)) ([0fa2ec8](https://github.com/jgsteeler/gsc-tracking/commit/0fa2ec8c94222b5f1aa8af1cf32495c8c63662e8))

## [0.1.11](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.10...frontend-v0.1.11) (2025-12-17)


### Features

* **customer:** implement CRUD operations with search and soft delete ([#48](https://github.com/jgsteeler/gsc-tracking/issues/48)) ([c23ae95](https://github.com/jgsteeler/gsc-tracking/commit/c23ae952920f918b3773ba8ab5fa1adf62f3281d))
* **customer:** implement customer management with CRUD operations ([#83](https://github.com/jgsteeler/gsc-tracking/issues/83)) ([217b831](https://github.com/jgsteeler/gsc-tracking/commit/217b831dd32e21ac0260cddbe6f4d0267c810c44))
* **infra:** add Netlify deployment with deploy previews as staging ([#55](https://github.com/jgsteeler/gsc-tracking/issues/55)) ([8795b4c](https://github.com/jgsteeler/gsc-tracking/commit/8795b4c194fbd99acd4fb92d2104038dc57b10eb))
* **job:** add customer creation from job screen ([#88](https://github.com/jgsteeler/gsc-tracking/issues/88)) ([4fb3086](https://github.com/jgsteeler/gsc-tracking/commit/4fb308601cadf9ed74e416a43341736998e4b93e))
* **job:** add job updates for tracking progress notes ([#96](https://github.com/jgsteeler/gsc-tracking/issues/96)) ([897adbf](https://github.com/jgsteeler/gsc-tracking/commit/897adbf40bd3413e7b5af85eb33465ab9d94f5f2))
* **job:** implement job management with CRUD operations and unit tests ([#68](https://github.com/jgsteeler/gsc-tracking/issues/68)) ([e0b98be](https://github.com/jgsteeler/gsc-tracking/commit/e0b98be69902e7c5889bb8f6fc8219e239b1f1ff))
* **test:** implement unit testing framework with CI/CD integration ([#59](https://github.com/jgsteeler/gsc-tracking/issues/59)) ([584d3a8](https://github.com/jgsteeler/gsc-tracking/commit/584d3a809e14466c236a3ec7afa53812ac9e750f))
* **validation:** implement comprehensive form validation with Zod and FluentValidation ([#101](https://github.com/jgsteeler/gsc-tracking/issues/101)) ([bf61ce9](https://github.com/jgsteeler/gsc-tracking/commit/bf61ce9baeb7955fe551b1f0260a32aa484bc08f))


### Bug Fixes

* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([eb690d2](https://github.com/jgsteeler/gsc-tracking/commit/eb690d2e97db09ad1587943271d1c11e28c01f82))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([fad2c07](https://github.com/jgsteeler/gsc-tracking/commit/fad2c07e181c1e4fda0d589ebc7c3afb473e8398))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([1f715b4](https://github.com/jgsteeler/gsc-tracking/commit/1f715b47b80834a728eb31aeed8acd6b35b980ae))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([71c3753](https://github.com/jgsteeler/gsc-tracking/commit/71c37535c2ab2c4f424cfda8b35468473b772ec8))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([ab1e9e2](https://github.com/jgsteeler/gsc-tracking/commit/ab1e9e274ed70e569aadc057ef35ba4b7be8f367))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([97b47ac](https://github.com/jgsteeler/gsc-tracking/commit/97b47ac5f1289a8c1958f7bba853669a31e57db7))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([e389db1](https://github.com/jgsteeler/gsc-tracking/commit/e389db1544d3cc10a39acfbef7bec959621199f4))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([0ac74a3](https://github.com/jgsteeler/gsc-tracking/commit/0ac74a33f2c104d866a01e9217d8afc683592b3e))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([b77b31e](https://github.com/jgsteeler/gsc-tracking/commit/b77b31e5df94b1621c56bbcf21b20fc659fe1f5c))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([a313eb3](https://github.com/jgsteeler/gsc-tracking/commit/a313eb3521369ae509d764900497963ce1b8470b))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([22015cb](https://github.com/jgsteeler/gsc-tracking/commit/22015cb985890b6c35201c0c32d55a7806cfc2d4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([5f52d99](https://github.com/jgsteeler/gsc-tracking/commit/5f52d99e85c0ee44b4847a168e897f774b6712f6))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([d561501](https://github.com/jgsteeler/gsc-tracking/commit/d561501c5e72afb4d82370be94b7149ec0f380a8))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([6b6c89e](https://github.com/jgsteeler/gsc-tracking/commit/6b6c89eb4c77efe902b44622af8ed79d4f2502c4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([14365d6](https://github.com/jgsteeler/gsc-tracking/commit/14365d60302c6433204f3e00c4c4655405f1faa0))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([8fd0a5a](https://github.com/jgsteeler/gsc-tracking/commit/8fd0a5a1f2fd1d8244a3d9c5d90ef343daa2ee2d))
* **ui:** make dialog forms and toast notifications fully opaque and visible ([#64](https://github.com/jgsteeler/gsc-tracking/issues/64)) ([b7c62da](https://github.com/jgsteeler/gsc-tracking/commit/b7c62dad97a70b580327202a81321e95ecf13669))

## [0.1.10](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.9...frontend-v0.1.10) (2025-12-16)


### Features

* **backend:** implement assembly version management with build metadata ([#108](https://github.com/jgsteeler/gsc-tracking/issues/108)) ([8b2c8d4](https://github.com/jgsteeler/gsc-tracking/commit/8b2c8d4448bd78903470de6490877f39a118e9d5))

## [0.1.9](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.8...frontend-v0.1.9) (2025-12-16)


### Features

* **validation:** implement comprehensive form validation with Zod and FluentValidation ([#101](https://github.com/jgsteeler/gsc-tracking/issues/101)) ([ba7d91c](https://github.com/jgsteeler/gsc-tracking/commit/ba7d91cb12a847aa32031f76b2db1489eadcafa5))

## [0.1.8](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.7...frontend-v0.1.8) (2025-12-15)


### Features

* **job:** add job updates for tracking progress notes ([#96](https://github.com/jgsteeler/gsc-tracking/issues/96)) ([24d4c28](https://github.com/jgsteeler/gsc-tracking/commit/24d4c2807d249ccb0108732537ea4a47270cc048))

## [0.1.7](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.6...frontend-v0.1.7) (2025-12-15)


### Bug Fixes

* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([4547c72](https://github.com/jgsteeler/gsc-tracking/commit/4547c72a8ea2cef05dc749f320cbb204537b3874))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([10ad171](https://github.com/jgsteeler/gsc-tracking/commit/10ad1714cf2d5790adaa7df4adb73f2e29f6f939))

## [0.1.6](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.5...frontend-v0.1.6) (2025-12-15)


### Features

* **job:** add customer creation from job screen ([#88](https://github.com/jgsteeler/gsc-tracking/issues/88)) ([3d1d2de](https://github.com/jgsteeler/gsc-tracking/commit/3d1d2de115985bba7eac1cc05c0cefcf51153c10))

## [0.1.5](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.4...frontend-v0.1.5) (2025-12-15)


### Features

* **customer:** implement customer management with CRUD operations ([#83](https://github.com/jgsteeler/gsc-tracking/issues/83)) ([6b3f1fb](https://github.com/jgsteeler/gsc-tracking/commit/6b3f1fb5325591b6f8e3404e3ad71230b7f6c0ca))

## [0.1.4](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.3...frontend-v0.1.4) (2025-12-12)


### Features

* **job:** implement job management with CRUD operations and unit tests ([#68](https://github.com/jgsteeler/gsc-tracking/issues/68)) ([e85bedf](https://github.com/jgsteeler/gsc-tracking/commit/e85bedfcb70e0c72c081d743c4c97c625c984c65))

## [0.1.3](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.2...frontend-v0.1.3) (2025-12-11)


### Features

* **test:** implement unit testing framework with CI/CD integration ([#59](https://github.com/jgsteeler/gsc-tracking/issues/59)) ([afe3ae4](https://github.com/jgsteeler/gsc-tracking/commit/afe3ae49ebb9fbe1b4b89a495bd5c65023ba0f53))


### Bug Fixes

* **ui:** make dialog forms and toast notifications fully opaque and visible ([#64](https://github.com/jgsteeler/gsc-tracking/issues/64)) ([5f72637](https://github.com/jgsteeler/gsc-tracking/commit/5f726373b0953705f1c03d98edd338cb4360ae17))

## [0.1.2](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.1...frontend-v0.1.2) (2025-12-10)


### Features

* **infra:** add Netlify deployment with deploy previews as staging ([#55](https://github.com/jgsteeler/gsc-tracking/issues/55)) ([39288bd](https://github.com/jgsteeler/gsc-tracking/commit/39288bd40517ac47dbd2913422e64169bc673680))

## [0.1.1](https://github.com/jgsteeler/gsc-tracking/compare/frontend-v0.1.0...frontend-v0.1.1) (2025-12-10)


### Features

* **customer:** implement CRUD operations with search and soft delete ([#48](https://github.com/jgsteeler/gsc-tracking/issues/48)) ([ec7a3a1](https://github.com/jgsteeler/gsc-tracking/commit/ec7a3a1fc9ace5d5062bcc8c1ed0e1c9b8b2ca70))

## Changelog

All notable changes to the frontend will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
