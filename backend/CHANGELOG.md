# Changelog

## [2.0.1](https://github.com/jgsteeler/gsc-tracking/compare/backend-v2.0.0...backend-v2.0.1) (2025-12-20)


### Bug Fixes

* **docs:** update Auth0 setup instructions for local development ([#161](https://github.com/jgsteeler/gsc-tracking/issues/161)) ([d59003e](https://github.com/jgsteeler/gsc-tracking/commit/d59003eca4c77e4c8ea65309938d146885881a01))

## [2.0.0](https://github.com/jgsteeler/gsc-tracking/compare/backend-v1.0.0...backend-v2.0.0) (2025-12-20)


### ⚠ BREAKING CHANGES

* Application has been deployed to production behind authentication. Marking this milestone as version 1.0.0 for both frontend and backend components.

### Features

* **api:** add Swagger documentation to backend API ([#84](https://github.com/jgsteeler/gsc-tracking/issues/84)) ([f861ef9](https://github.com/jgsteeler/gsc-tracking/commit/f861ef958472d1c404d01025e533caa498af3a31))
* **auth:** implement Auth0 authentication for backend and frontend  ([#129](https://github.com/jgsteeler/gsc-tracking/issues/129)) ([472e2f4](https://github.com/jgsteeler/gsc-tracking/commit/472e2f46c16a1f7864d5edf2cec6615ad01bb56f))
* **backend:** implement assembly version management with build metadata ([#108](https://github.com/jgsteeler/gsc-tracking/issues/108)) ([71dfde4](https://github.com/jgsteeler/gsc-tracking/commit/71dfde4f4a56202be602031216b8801f3dd06e92))
* **customer:** implement CRUD operations with search and soft delete ([#48](https://github.com/jgsteeler/gsc-tracking/issues/48)) ([c23ae95](https://github.com/jgsteeler/gsc-tracking/commit/c23ae952920f918b3773ba8ab5fa1adf62f3281d))
* **customer:** implement customer management with CRUD operations ([#83](https://github.com/jgsteeler/gsc-tracking/issues/83)) ([217b831](https://github.com/jgsteeler/gsc-tracking/commit/217b831dd32e21ac0260cddbe6f4d0267c810c44))
* **docs:** add setup instructions and business management app analysis ([#140](https://github.com/jgsteeler/gsc-tracking/issues/140)) ([de7eb3a](https://github.com/jgsteeler/gsc-tracking/commit/de7eb3ae8424c374de59d7968a42916800b03c5b))
* **expense:** add backend expense tracking system with CRUD operations ([#121](https://github.com/jgsteeler/gsc-tracking/issues/121)) ([0fa2ec8](https://github.com/jgsteeler/gsc-tracking/commit/0fa2ec8c94222b5f1aa8af1cf32495c8c63662e8))
* **expense:** add job validation to UpdateExpense endpoint ([#124](https://github.com/jgsteeler/gsc-tracking/issues/124)) ([4b99ddc](https://github.com/jgsteeler/gsc-tracking/commit/4b99ddc372d3dd5c1ec63f468b169b9031ed9bc1))
* **infra:** add Fly.io deployment with GitHub Flow and staging environment ([#50](https://github.com/jgsteeler/gsc-tracking/issues/50)) ([9230834](https://github.com/jgsteeler/gsc-tracking/commit/9230834908b1a687a579e2b515457d1d7abf3365))
* **infra:** add PostgreSQL support and comprehensive database documentation ([#67](https://github.com/jgsteeler/gsc-tracking/issues/67)) ([1dc741c](https://github.com/jgsteeler/gsc-tracking/commit/1dc741c399fbba5e9a370f8a9cbc889c282c7883))
* **infra:** implement pattern-based CORS for Netlify deploy previews ([#112](https://github.com/jgsteeler/gsc-tracking/issues/112)) ([a6008a7](https://github.com/jgsteeler/gsc-tracking/commit/a6008a754d48c2a8fb2b48e9091c127b70a4724c))
* **job:** add job updates for tracking progress notes ([#96](https://github.com/jgsteeler/gsc-tracking/issues/96)) ([897adbf](https://github.com/jgsteeler/gsc-tracking/commit/897adbf40bd3413e7b5af85eb33465ab9d94f5f2))
* **job:** implement job management with CRUD operations and unit tests ([#68](https://github.com/jgsteeler/gsc-tracking/issues/68)) ([e0b98be](https://github.com/jgsteeler/gsc-tracking/commit/e0b98be69902e7c5889bb8f6fc8219e239b1f1ff))
* **test:** implement unit testing framework with CI/CD integration ([#59](https://github.com/jgsteeler/gsc-tracking/issues/59)) ([584d3a8](https://github.com/jgsteeler/gsc-tracking/commit/584d3a809e14466c236a3ec7afa53812ac9e750f))
* **validation:** implement comprehensive form validation with Zod and FluentValidation ([#101](https://github.com/jgsteeler/gsc-tracking/issues/101)) ([bf61ce9](https://github.com/jgsteeler/gsc-tracking/commit/bf61ce9baeb7955fe551b1f0260a32aa484bc08f))


### Bug Fixes

* **db:** ef core connection issues ([#89](https://github.com/jgsteeler/gsc-tracking/issues/89)) ([bca711e](https://github.com/jgsteeler/gsc-tracking/commit/bca711e71a7bf5c72bb21e8b523627fbdb84875f))
* **db:** improve EF Core PostgreSQL connection handling for Neon ([#85](https://github.com/jgsteeler/gsc-tracking/issues/85)) ([865a2d8](https://github.com/jgsteeler/gsc-tracking/commit/865a2d8b3e49fe39e9a8d0d79c01381a1a1b5dd3))
* **db:** infra set up staging database ([#87](https://github.com/jgsteeler/gsc-tracking/issues/87)) ([b06883b](https://github.com/jgsteeler/gsc-tracking/commit/b06883bf7db8f75fdc7a99c3ff551f3b3fbc26bb))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([eb690d2](https://github.com/jgsteeler/gsc-tracking/commit/eb690d2e97db09ad1587943271d1c11e28c01f82))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([fad2c07](https://github.com/jgsteeler/gsc-tracking/commit/fad2c07e181c1e4fda0d589ebc7c3afb473e8398))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([1f715b4](https://github.com/jgsteeler/gsc-tracking/commit/1f715b47b80834a728eb31aeed8acd6b35b980ae))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([71c3753](https://github.com/jgsteeler/gsc-tracking/commit/71c37535c2ab2c4f424cfda8b35468473b772ec8))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([ab1e9e2](https://github.com/jgsteeler/gsc-tracking/commit/ab1e9e274ed70e569aadc057ef35ba4b7be8f367))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([97b47ac](https://github.com/jgsteeler/gsc-tracking/commit/97b47ac5f1289a8c1958f7bba853669a31e57db7))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([e389db1](https://github.com/jgsteeler/gsc-tracking/commit/e389db1544d3cc10a39acfbef7bec959621199f4))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([0ac74a3](https://github.com/jgsteeler/gsc-tracking/commit/0ac74a33f2c104d866a01e9217d8afc683592b3e))
* **db:** restore PostgreSQL resilience and fix cross-database compatibility ([#90](https://github.com/jgsteeler/gsc-tracking/issues/90)) ([1770f49](https://github.com/jgsteeler/gsc-tracking/commit/1770f4966b45d62b55dfcb6630b73b402837bfa8))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([b77b31e](https://github.com/jgsteeler/gsc-tracking/commit/b77b31e5df94b1621c56bbcf21b20fc659fe1f5c))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([a313eb3](https://github.com/jgsteeler/gsc-tracking/commit/a313eb3521369ae509d764900497963ce1b8470b))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([22015cb](https://github.com/jgsteeler/gsc-tracking/commit/22015cb985890b6c35201c0c32d55a7806cfc2d4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([5f52d99](https://github.com/jgsteeler/gsc-tracking/commit/5f52d99e85c0ee44b4847a168e897f774b6712f6))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([d561501](https://github.com/jgsteeler/gsc-tracking/commit/d561501c5e72afb4d82370be94b7149ec0f380a8))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([6b6c89e](https://github.com/jgsteeler/gsc-tracking/commit/6b6c89eb4c77efe902b44622af8ed79d4f2502c4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([14365d6](https://github.com/jgsteeler/gsc-tracking/commit/14365d60302c6433204f3e00c4c4655405f1faa0))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([8fd0a5a](https://github.com/jgsteeler/gsc-tracking/commit/8fd0a5a1f2fd1d8244a3d9c5d90ef343daa2ee2d))
* **expense:** add job validation before expense deletion ([#123](https://github.com/jgsteeler/gsc-tracking/issues/123)) ([a600c33](https://github.com/jgsteeler/gsc-tracking/commit/a600c331dcaa1d962d633ba31738196cb391a310))
* simplify conditions in Fly.io deployment workflows and update configuration files ([#54](https://github.com/jgsteeler/gsc-tracking/issues/54)) ([b5fa2be](https://github.com/jgsteeler/gsc-tracking/commit/b5fa2be91126333860abc905962d8dc49a006c62))


### Miscellaneous Chores

* release version 1.0.0 for production deployment ([#154](https://github.com/jgsteeler/gsc-tracking/issues/154)) ([998a6f3](https://github.com/jgsteeler/gsc-tracking/commit/998a6f346104c56f1f178865f161b06793264fbc))

## [1.0.0](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.18...backend-v1.0.0) (2025-12-19)


### ⚠ BREAKING CHANGES

* Application has been deployed to production behind authentication. Marking this milestone as version 1.0.0 for both frontend and backend components.

### Features

* **api:** add Swagger documentation to backend API ([#84](https://github.com/jgsteeler/gsc-tracking/issues/84)) ([f861ef9](https://github.com/jgsteeler/gsc-tracking/commit/f861ef958472d1c404d01025e533caa498af3a31))
* **auth:** implement Auth0 authentication for backend and frontend  ([#129](https://github.com/jgsteeler/gsc-tracking/issues/129)) ([472e2f4](https://github.com/jgsteeler/gsc-tracking/commit/472e2f46c16a1f7864d5edf2cec6615ad01bb56f))
* **backend:** implement assembly version management with build metadata ([#108](https://github.com/jgsteeler/gsc-tracking/issues/108)) ([71dfde4](https://github.com/jgsteeler/gsc-tracking/commit/71dfde4f4a56202be602031216b8801f3dd06e92))
* **customer:** implement CRUD operations with search and soft delete ([#48](https://github.com/jgsteeler/gsc-tracking/issues/48)) ([c23ae95](https://github.com/jgsteeler/gsc-tracking/commit/c23ae952920f918b3773ba8ab5fa1adf62f3281d))
* **customer:** implement customer management with CRUD operations ([#83](https://github.com/jgsteeler/gsc-tracking/issues/83)) ([217b831](https://github.com/jgsteeler/gsc-tracking/commit/217b831dd32e21ac0260cddbe6f4d0267c810c44))
* **docs:** add setup instructions and business management app analysis ([#140](https://github.com/jgsteeler/gsc-tracking/issues/140)) ([de7eb3a](https://github.com/jgsteeler/gsc-tracking/commit/de7eb3ae8424c374de59d7968a42916800b03c5b))
* **expense:** add backend expense tracking system with CRUD operations ([#121](https://github.com/jgsteeler/gsc-tracking/issues/121)) ([0fa2ec8](https://github.com/jgsteeler/gsc-tracking/commit/0fa2ec8c94222b5f1aa8af1cf32495c8c63662e8))
* **expense:** add job validation to UpdateExpense endpoint ([#124](https://github.com/jgsteeler/gsc-tracking/issues/124)) ([4b99ddc](https://github.com/jgsteeler/gsc-tracking/commit/4b99ddc372d3dd5c1ec63f468b169b9031ed9bc1))
* **infra:** add Fly.io deployment with GitHub Flow and staging environment ([#50](https://github.com/jgsteeler/gsc-tracking/issues/50)) ([9230834](https://github.com/jgsteeler/gsc-tracking/commit/9230834908b1a687a579e2b515457d1d7abf3365))
* **infra:** add PostgreSQL support and comprehensive database documentation ([#67](https://github.com/jgsteeler/gsc-tracking/issues/67)) ([1dc741c](https://github.com/jgsteeler/gsc-tracking/commit/1dc741c399fbba5e9a370f8a9cbc889c282c7883))
* **infra:** implement pattern-based CORS for Netlify deploy previews ([#112](https://github.com/jgsteeler/gsc-tracking/issues/112)) ([a6008a7](https://github.com/jgsteeler/gsc-tracking/commit/a6008a754d48c2a8fb2b48e9091c127b70a4724c))
* **job:** add job updates for tracking progress notes ([#96](https://github.com/jgsteeler/gsc-tracking/issues/96)) ([897adbf](https://github.com/jgsteeler/gsc-tracking/commit/897adbf40bd3413e7b5af85eb33465ab9d94f5f2))
* **job:** implement job management with CRUD operations and unit tests ([#68](https://github.com/jgsteeler/gsc-tracking/issues/68)) ([e0b98be](https://github.com/jgsteeler/gsc-tracking/commit/e0b98be69902e7c5889bb8f6fc8219e239b1f1ff))
* **test:** implement unit testing framework with CI/CD integration ([#59](https://github.com/jgsteeler/gsc-tracking/issues/59)) ([584d3a8](https://github.com/jgsteeler/gsc-tracking/commit/584d3a809e14466c236a3ec7afa53812ac9e750f))
* **validation:** implement comprehensive form validation with Zod and FluentValidation ([#101](https://github.com/jgsteeler/gsc-tracking/issues/101)) ([bf61ce9](https://github.com/jgsteeler/gsc-tracking/commit/bf61ce9baeb7955fe551b1f0260a32aa484bc08f))


### Bug Fixes

* **db:** ef core connection issues ([#89](https://github.com/jgsteeler/gsc-tracking/issues/89)) ([bca711e](https://github.com/jgsteeler/gsc-tracking/commit/bca711e71a7bf5c72bb21e8b523627fbdb84875f))
* **db:** improve EF Core PostgreSQL connection handling for Neon ([#85](https://github.com/jgsteeler/gsc-tracking/issues/85)) ([865a2d8](https://github.com/jgsteeler/gsc-tracking/commit/865a2d8b3e49fe39e9a8d0d79c01381a1a1b5dd3))
* **db:** infra set up staging database ([#87](https://github.com/jgsteeler/gsc-tracking/issues/87)) ([b06883b](https://github.com/jgsteeler/gsc-tracking/commit/b06883bf7db8f75fdc7a99c3ff551f3b3fbc26bb))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([eb690d2](https://github.com/jgsteeler/gsc-tracking/commit/eb690d2e97db09ad1587943271d1c11e28c01f82))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([fad2c07](https://github.com/jgsteeler/gsc-tracking/commit/fad2c07e181c1e4fda0d589ebc7c3afb473e8398))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([1f715b4](https://github.com/jgsteeler/gsc-tracking/commit/1f715b47b80834a728eb31aeed8acd6b35b980ae))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([71c3753](https://github.com/jgsteeler/gsc-tracking/commit/71c37535c2ab2c4f424cfda8b35468473b772ec8))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([ab1e9e2](https://github.com/jgsteeler/gsc-tracking/commit/ab1e9e274ed70e569aadc057ef35ba4b7be8f367))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([97b47ac](https://github.com/jgsteeler/gsc-tracking/commit/97b47ac5f1289a8c1958f7bba853669a31e57db7))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([e389db1](https://github.com/jgsteeler/gsc-tracking/commit/e389db1544d3cc10a39acfbef7bec959621199f4))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([0ac74a3](https://github.com/jgsteeler/gsc-tracking/commit/0ac74a33f2c104d866a01e9217d8afc683592b3e))
* **db:** restore PostgreSQL resilience and fix cross-database compatibility ([#90](https://github.com/jgsteeler/gsc-tracking/issues/90)) ([1770f49](https://github.com/jgsteeler/gsc-tracking/commit/1770f4966b45d62b55dfcb6630b73b402837bfa8))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([b77b31e](https://github.com/jgsteeler/gsc-tracking/commit/b77b31e5df94b1621c56bbcf21b20fc659fe1f5c))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([a313eb3](https://github.com/jgsteeler/gsc-tracking/commit/a313eb3521369ae509d764900497963ce1b8470b))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([22015cb](https://github.com/jgsteeler/gsc-tracking/commit/22015cb985890b6c35201c0c32d55a7806cfc2d4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([5f52d99](https://github.com/jgsteeler/gsc-tracking/commit/5f52d99e85c0ee44b4847a168e897f774b6712f6))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([d561501](https://github.com/jgsteeler/gsc-tracking/commit/d561501c5e72afb4d82370be94b7149ec0f380a8))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([6b6c89e](https://github.com/jgsteeler/gsc-tracking/commit/6b6c89eb4c77efe902b44622af8ed79d4f2502c4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([14365d6](https://github.com/jgsteeler/gsc-tracking/commit/14365d60302c6433204f3e00c4c4655405f1faa0))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([8fd0a5a](https://github.com/jgsteeler/gsc-tracking/commit/8fd0a5a1f2fd1d8244a3d9c5d90ef343daa2ee2d))
* **expense:** add job validation before expense deletion ([#123](https://github.com/jgsteeler/gsc-tracking/issues/123)) ([a600c33](https://github.com/jgsteeler/gsc-tracking/commit/a600c331dcaa1d962d633ba31738196cb391a310))
* simplify conditions in Fly.io deployment workflows and update configuration files ([#54](https://github.com/jgsteeler/gsc-tracking/issues/54)) ([b5fa2be](https://github.com/jgsteeler/gsc-tracking/commit/b5fa2be91126333860abc905962d8dc49a006c62))


### Miscellaneous Chores

* release version 1.0.0 for production deployment ([#154](https://github.com/jgsteeler/gsc-tracking/issues/154)) ([998a6f3](https://github.com/jgsteeler/gsc-tracking/commit/998a6f346104c56f1f178865f161b06793264fbc))

## [0.1.18](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.17...backend-v0.1.18) (2025-12-18)


### Features

* **docs:** add setup instructions and business management app analysis ([#140](https://github.com/jgsteeler/gsc-tracking/issues/140)) ([de7eb3a](https://github.com/jgsteeler/gsc-tracking/commit/de7eb3ae8424c374de59d7968a42916800b03c5b))

## [0.1.17](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.16...backend-v0.1.17) (2025-12-18)


### Features

* **auth:** implement Auth0 authentication for backend and frontend  ([#129](https://github.com/jgsteeler/gsc-tracking/issues/129)) ([472e2f4](https://github.com/jgsteeler/gsc-tracking/commit/472e2f46c16a1f7864d5edf2cec6615ad01bb56f))

## [0.1.16](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.15...backend-v0.1.16) (2025-12-18)


### Features

* **expense:** add job validation to UpdateExpense endpoint ([#124](https://github.com/jgsteeler/gsc-tracking/issues/124)) ([4b99ddc](https://github.com/jgsteeler/gsc-tracking/commit/4b99ddc372d3dd5c1ec63f468b169b9031ed9bc1))

## [0.1.15](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.14...backend-v0.1.15) (2025-12-17)


### Features

* **expense:** add backend expense tracking system with CRUD operations ([#121](https://github.com/jgsteeler/gsc-tracking/issues/121)) ([0fa2ec8](https://github.com/jgsteeler/gsc-tracking/commit/0fa2ec8c94222b5f1aa8af1cf32495c8c63662e8))


### Bug Fixes

* **expense:** add job validation before expense deletion ([#123](https://github.com/jgsteeler/gsc-tracking/issues/123)) ([a600c33](https://github.com/jgsteeler/gsc-tracking/commit/a600c331dcaa1d962d633ba31738196cb391a310))

## [0.1.14](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.13...backend-v0.1.14) (2025-12-17)


### Features

* **api:** add Swagger documentation to backend API ([#84](https://github.com/jgsteeler/gsc-tracking/issues/84)) ([f861ef9](https://github.com/jgsteeler/gsc-tracking/commit/f861ef958472d1c404d01025e533caa498af3a31))
* **backend:** implement assembly version management with build metadata ([#108](https://github.com/jgsteeler/gsc-tracking/issues/108)) ([71dfde4](https://github.com/jgsteeler/gsc-tracking/commit/71dfde4f4a56202be602031216b8801f3dd06e92))
* **customer:** implement CRUD operations with search and soft delete ([#48](https://github.com/jgsteeler/gsc-tracking/issues/48)) ([c23ae95](https://github.com/jgsteeler/gsc-tracking/commit/c23ae952920f918b3773ba8ab5fa1adf62f3281d))
* **customer:** implement customer management with CRUD operations ([#83](https://github.com/jgsteeler/gsc-tracking/issues/83)) ([217b831](https://github.com/jgsteeler/gsc-tracking/commit/217b831dd32e21ac0260cddbe6f4d0267c810c44))
* **infra:** add Fly.io deployment with GitHub Flow and staging environment ([#50](https://github.com/jgsteeler/gsc-tracking/issues/50)) ([9230834](https://github.com/jgsteeler/gsc-tracking/commit/9230834908b1a687a579e2b515457d1d7abf3365))
* **infra:** add PostgreSQL support and comprehensive database documentation ([#67](https://github.com/jgsteeler/gsc-tracking/issues/67)) ([1dc741c](https://github.com/jgsteeler/gsc-tracking/commit/1dc741c399fbba5e9a370f8a9cbc889c282c7883))
* **infra:** implement pattern-based CORS for Netlify deploy previews ([#112](https://github.com/jgsteeler/gsc-tracking/issues/112)) ([a6008a7](https://github.com/jgsteeler/gsc-tracking/commit/a6008a754d48c2a8fb2b48e9091c127b70a4724c))
* **job:** add job updates for tracking progress notes ([#96](https://github.com/jgsteeler/gsc-tracking/issues/96)) ([897adbf](https://github.com/jgsteeler/gsc-tracking/commit/897adbf40bd3413e7b5af85eb33465ab9d94f5f2))
* **job:** implement job management with CRUD operations and unit tests ([#68](https://github.com/jgsteeler/gsc-tracking/issues/68)) ([e0b98be](https://github.com/jgsteeler/gsc-tracking/commit/e0b98be69902e7c5889bb8f6fc8219e239b1f1ff))
* **test:** implement unit testing framework with CI/CD integration ([#59](https://github.com/jgsteeler/gsc-tracking/issues/59)) ([584d3a8](https://github.com/jgsteeler/gsc-tracking/commit/584d3a809e14466c236a3ec7afa53812ac9e750f))
* **validation:** implement comprehensive form validation with Zod and FluentValidation ([#101](https://github.com/jgsteeler/gsc-tracking/issues/101)) ([bf61ce9](https://github.com/jgsteeler/gsc-tracking/commit/bf61ce9baeb7955fe551b1f0260a32aa484bc08f))


### Bug Fixes

* **db:** ef core connection issues ([#89](https://github.com/jgsteeler/gsc-tracking/issues/89)) ([bca711e](https://github.com/jgsteeler/gsc-tracking/commit/bca711e71a7bf5c72bb21e8b523627fbdb84875f))
* **db:** improve EF Core PostgreSQL connection handling for Neon ([#85](https://github.com/jgsteeler/gsc-tracking/issues/85)) ([865a2d8](https://github.com/jgsteeler/gsc-tracking/commit/865a2d8b3e49fe39e9a8d0d79c01381a1a1b5dd3))
* **db:** infra set up staging database ([#87](https://github.com/jgsteeler/gsc-tracking/issues/87)) ([b06883b](https://github.com/jgsteeler/gsc-tracking/commit/b06883bf7db8f75fdc7a99c3ff551f3b3fbc26bb))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([eb690d2](https://github.com/jgsteeler/gsc-tracking/commit/eb690d2e97db09ad1587943271d1c11e28c01f82))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([fad2c07](https://github.com/jgsteeler/gsc-tracking/commit/fad2c07e181c1e4fda0d589ebc7c3afb473e8398))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([1f715b4](https://github.com/jgsteeler/gsc-tracking/commit/1f715b47b80834a728eb31aeed8acd6b35b980ae))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([71c3753](https://github.com/jgsteeler/gsc-tracking/commit/71c37535c2ab2c4f424cfda8b35468473b772ec8))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([ab1e9e2](https://github.com/jgsteeler/gsc-tracking/commit/ab1e9e274ed70e569aadc057ef35ba4b7be8f367))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([97b47ac](https://github.com/jgsteeler/gsc-tracking/commit/97b47ac5f1289a8c1958f7bba853669a31e57db7))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([e389db1](https://github.com/jgsteeler/gsc-tracking/commit/e389db1544d3cc10a39acfbef7bec959621199f4))
* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([0ac74a3](https://github.com/jgsteeler/gsc-tracking/commit/0ac74a33f2c104d866a01e9217d8afc683592b3e))
* **db:** restore PostgreSQL resilience and fix cross-database compatibility ([#90](https://github.com/jgsteeler/gsc-tracking/issues/90)) ([1770f49](https://github.com/jgsteeler/gsc-tracking/commit/1770f4966b45d62b55dfcb6630b73b402837bfa8))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([b77b31e](https://github.com/jgsteeler/gsc-tracking/commit/b77b31e5df94b1621c56bbcf21b20fc659fe1f5c))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([a313eb3](https://github.com/jgsteeler/gsc-tracking/commit/a313eb3521369ae509d764900497963ce1b8470b))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([22015cb](https://github.com/jgsteeler/gsc-tracking/commit/22015cb985890b6c35201c0c32d55a7806cfc2d4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([5f52d99](https://github.com/jgsteeler/gsc-tracking/commit/5f52d99e85c0ee44b4847a168e897f774b6712f6))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([d561501](https://github.com/jgsteeler/gsc-tracking/commit/d561501c5e72afb4d82370be94b7149ec0f380a8))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([6b6c89e](https://github.com/jgsteeler/gsc-tracking/commit/6b6c89eb4c77efe902b44622af8ed79d4f2502c4))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([14365d6](https://github.com/jgsteeler/gsc-tracking/commit/14365d60302c6433204f3e00c4c4655405f1faa0))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([8fd0a5a](https://github.com/jgsteeler/gsc-tracking/commit/8fd0a5a1f2fd1d8244a3d9c5d90ef343daa2ee2d))
* simplify conditions in Fly.io deployment workflows and update configuration files ([#54](https://github.com/jgsteeler/gsc-tracking/issues/54)) ([b5fa2be](https://github.com/jgsteeler/gsc-tracking/commit/b5fa2be91126333860abc905962d8dc49a006c62))

## [0.1.13](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.12...backend-v0.1.13) (2025-12-16)


### Features

* **infra:** implement pattern-based CORS for Netlify deploy previews ([#112](https://github.com/jgsteeler/gsc-tracking/issues/112)) ([9caa550](https://github.com/jgsteeler/gsc-tracking/commit/9caa5504df6be6e579e0438063d3178ddfbfff7b))

## [0.1.12](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.11...backend-v0.1.12) (2025-12-16)


### Features

* **backend:** implement assembly version management with build metadata ([#108](https://github.com/jgsteeler/gsc-tracking/issues/108)) ([8b2c8d4](https://github.com/jgsteeler/gsc-tracking/commit/8b2c8d4448bd78903470de6490877f39a118e9d5))

## [0.1.11](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.10...backend-v0.1.11) (2025-12-16)


### Features

* **validation:** implement comprehensive form validation with Zod and FluentValidation ([#101](https://github.com/jgsteeler/gsc-tracking/issues/101)) ([ba7d91c](https://github.com/jgsteeler/gsc-tracking/commit/ba7d91cb12a847aa32031f76b2db1489eadcafa5))

## [0.1.10](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.9...backend-v0.1.10) (2025-12-15)


### Features

* **job:** add job updates for tracking progress notes ([#96](https://github.com/jgsteeler/gsc-tracking/issues/96)) ([24d4c28](https://github.com/jgsteeler/gsc-tracking/commit/24d4c2807d249ccb0108732537ea4a47270cc048))

## [0.1.9](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.8...backend-v0.1.9) (2025-12-15)


### Bug Fixes

* **db:** remove obsolete TrustServerCertificate parameter ([#94](https://github.com/jgsteeler/gsc-tracking/issues/94)) ([4547c72](https://github.com/jgsteeler/gsc-tracking/commit/4547c72a8ea2cef05dc749f320cbb204537b3874))
* **db:** validate database URL format before parsing credentials ([#93](https://github.com/jgsteeler/gsc-tracking/issues/93)) ([10ad171](https://github.com/jgsteeler/gsc-tracking/commit/10ad1714cf2d5790adaa7df4adb73f2e29f6f939))

## [0.1.8](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.7...backend-v0.1.8) (2025-12-15)


### Bug Fixes

* **db:** ef core connection issues ([#89](https://github.com/jgsteeler/gsc-tracking/issues/89)) ([ceda0e5](https://github.com/jgsteeler/gsc-tracking/commit/ceda0e5a35f2232058c4ea994e9aa3ffaa75572b))
* **db:** infra set up staging database ([#87](https://github.com/jgsteeler/gsc-tracking/issues/87)) ([82519f0](https://github.com/jgsteeler/gsc-tracking/commit/82519f00e6abc71a6643af703363093941e439d8))
* **db:** restore PostgreSQL resilience and fix cross-database compatibility ([#90](https://github.com/jgsteeler/gsc-tracking/issues/90)) ([be7d693](https://github.com/jgsteeler/gsc-tracking/commit/be7d69308254310bc1dcf7eb937db87e40485b29))

## [0.1.7](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.6...backend-v0.1.7) (2025-12-15)


### Features

* **api:** add Swagger documentation to backend API ([#84](https://github.com/jgsteeler/gsc-tracking/issues/84)) ([a3a58fe](https://github.com/jgsteeler/gsc-tracking/commit/a3a58fefc355774aea0061fa08043c6887b06a3b))
* **customer:** implement customer management with CRUD operations ([#83](https://github.com/jgsteeler/gsc-tracking/issues/83)) ([6b3f1fb](https://github.com/jgsteeler/gsc-tracking/commit/6b3f1fb5325591b6f8e3404e3ad71230b7f6c0ca))


### Bug Fixes

* **db:** improve EF Core PostgreSQL connection handling for Neon ([#85](https://github.com/jgsteeler/gsc-tracking/issues/85)) ([1b4224a](https://github.com/jgsteeler/gsc-tracking/commit/1b4224a8b6acc2126f08c47bf796547eccd11b41))

## [0.1.6](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.5...backend-v0.1.6) (2025-12-12)


### Features

* **job:** implement job management with CRUD operations and unit tests ([#68](https://github.com/jgsteeler/gsc-tracking/issues/68)) ([e85bedf](https://github.com/jgsteeler/gsc-tracking/commit/e85bedfcb70e0c72c081d743c4c97c625c984c65))

## [0.1.5](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.4...backend-v0.1.5) (2025-12-11)


### Features

* **infra:** add PostgreSQL support and comprehensive database documentation ([#67](https://github.com/jgsteeler/gsc-tracking/issues/67)) ([4a2cab9](https://github.com/jgsteeler/gsc-tracking/commit/4a2cab917bca1f1e7b34362588226d26395fce38))

## [0.1.4](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.3...backend-v0.1.4) (2025-12-11)


### Features

* **test:** implement unit testing framework with CI/CD integration ([#59](https://github.com/jgsteeler/gsc-tracking/issues/59)) ([afe3ae4](https://github.com/jgsteeler/gsc-tracking/commit/afe3ae49ebb9fbe1b4b89a495bd5c65023ba0f53))

## [0.1.3](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.2...backend-v0.1.3) (2025-12-10)


### Bug Fixes

* simplify conditions in Fly.io deployment workflows and update configuration files ([#54](https://github.com/jgsteeler/gsc-tracking/issues/54)) ([027e362](https://github.com/jgsteeler/gsc-tracking/commit/027e362b7a30b2930f30dbc17307a473ad746960))

## [0.1.2](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.1...backend-v0.1.2) (2025-12-10)


### Features

* **infra:** add Fly.io deployment with GitHub Flow and staging environment ([#50](https://github.com/jgsteeler/gsc-tracking/issues/50)) ([4e1930f](https://github.com/jgsteeler/gsc-tracking/commit/4e1930ff228e6bc1574a2a4a39f424b3e326145a))

## [0.1.1](https://github.com/jgsteeler/gsc-tracking/compare/backend-v0.1.0...backend-v0.1.1) (2025-12-10)


### Features

* **customer:** implement CRUD operations with search and soft delete ([#48](https://github.com/jgsteeler/gsc-tracking/issues/48)) ([ec7a3a1](https://github.com/jgsteeler/gsc-tracking/commit/ec7a3a1fc9ace5d5062bcc8c1ed0e1c9b8b2ca70))

## Changelog

All notable changes to the backend will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
