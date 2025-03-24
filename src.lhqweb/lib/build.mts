import path from 'node:path';
import util from 'node:util';
import { exec } from 'node:child_process';
import { build } from 'tsup';
import fse from 'fs-extra';
import type { IPackageJson } from 'package-json-type';

const execAsync = util.promisify(exec);

(async () => {
    try {
        // await buildLib();
        // await buildCli();
        // await buildDts();
        // await execAsync('pnpm version patch');
        // await copyPackageJson();

        await Promise.all([
            buildLib(),
            buildCli(),
            buildDts(),
            copyPackageJson()
        ]);

    } catch (error) {
        console.error('Build failed: ', error);
    }
})();

async function buildLib() {
    console.log('Building library...');

    await build({
        entry: ['src/index.mts'],
        outDir: '',
        format: 'iife',
        silent: true,
        globalName: 'LhqGenerators',
        target: 'es2015',
        minify: false,
        platform: 'browser',
        sourcemap: false,
        dts: false,
        clean: false,
        tsconfig: 'tsconfig.json',
        esbuildOptions(options, _) {
            options.target = 'es2015';
            options.outfile = 'dist/index.js';
            options.supported = {
                'arrow': true,
                'const-and-let': true,
                'for-of': true,
                'array-spread': true,
                'destructuring': true,
                'default-argument': true,
                'template-literal': true,
            };
        },
    });
}

async function buildDts() {
    console.log('Building dts ...');

    await build({
        entry: ['src/index.mts'],
        outDir: '',
        dts: true,
        clean: false,
        silent: true,
        tsconfig: 'tsconfig.dts.json',
        esbuildOptions(options) {
            options.outfile = 'dist/index.d.ts';
        }
    });
}

async function buildCli() {
    console.log('Building CLI...');

    await build({
        entry: ['build/cli.mts'],
        outDir: '',
        format: 'esm',
        silent: true,
        bundle: true,
        skipNodeModulesBundle: true,
        platform: 'node',
        external: ['path', 'fs', 'os'],
        minify: false,
        splitting: false,
        sourcemap: false,
        dts: false,
        clean: false,
        tsconfig: 'tsconfig.build.json',
        esbuildOptions(options) {
            options.outfile = 'dist/cli.mjs';
            options.platform = 'node';
            options.external = ['path', 'fs', 'os'];
        },
    });
}

async function copyPackageJson() {
    execAsync('pnpm version patch');

    const packageJson: Partial<IPackageJson> = await fse.readJson('package.json', { encoding: 'utf-8' });

    const devDependenciesToCopy = [
        'commander', 'fs-extra', 'glob', 'picocolors'
    ];

    const devDependencies = Object.fromEntries(
        Object.entries(packageJson.devDependencies!).filter(([key]) => devDependenciesToCopy.includes(key))
    );

    const newPackageJson: Partial<IPackageJson> = {
        name: packageJson.name,
        version: packageJson.version,
        author: packageJson.author,
        description: packageJson.description,
        engines: packageJson.engines,
        dependencies: devDependencies,
        peerDependencies: packageJson.peerDependencies,
        main: 'index.mjs',
        types: 'index.d.ts',
        type: 'module',
        bin: {
            lhqcmd: 'cli.mjs'
        }
    }

    await fse.writeJson(path.join('dist', 'package.json'),
        newPackageJson, { encoding: 'utf-8', spaces: 2 });
}