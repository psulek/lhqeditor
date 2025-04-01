import path from 'node:path';
import util from 'node:util';
import { exec } from 'node:child_process';

import { build, type Options } from 'tsup';
//type TsupBuildOptions = Parameters<typeof build>[0];

import fse from 'fs-extra';
import type { IPackageJson } from 'package-json-type';
import pc from "picocolors"

import { generateLhqSchema } from './src/generatorUtils';

const execAsync = util.promisify(exec);

const distFolder = path.join(__dirname, 'dist');

let packageJson: Partial<IPackageJson>;

type EsBuildOptions = Parameters<NonNullable<Options['esbuildOptions']>>[0];


(async () => {
    try {
        await fse.ensureDir(distFolder);

        await preparePackageVersion();

        await Promise.all([
            buildLib('browser'),
            buildLib('cjs'),
            buildLib('esm'),
            buildCli(),
            buildDts(),
            copyPackageJson(),
            genLhqSchema()
        ]);

    } catch (error) {
        console.error('Build failed: ', error, error.stack);
    }
})();

function updateBuildOptions(opts: EsBuildOptions): void {
    opts.define = {
        'PKG_VERSION': `'${packageJson.version}'`,
    };
}

async function preparePackageVersion() {
    await execAsync('pnpm version patch', { cwd: __dirname });

    const sourcePackageFile = path.join(__dirname, 'package.json');
    packageJson = await fse.readJson(sourcePackageFile, { encoding: 'utf-8' });

    console.log('Updated local version to ' + pc.blueBright(packageJson.version));

}

async function buildLib(type: 'browser' | 'cjs' | 'esm') {
    const isBrowser = type === 'browser';
    const isEsm = type === 'esm';

    const subdir = type === 'cjs' ? '' : type;
    let outfile = path.join('dist', subdir, 'index.' + (isEsm ? 'mjs' : 'js'));
    console.log(`Building library to ${pc.blueBright(outfile)} ...`);
    outfile = path.join(__dirname, outfile);

    const target = isBrowser ? 'es2015' : 'es2017';

    await build({
        entry: ['src/index.ts'],
        outDir: '',
        format: isBrowser ? 'iife' : type,
        silent: true,
        globalName: isBrowser ? 'LhqGenerators' : undefined,
        target: target,
        minify: false,
        splitting: isEsm ? false : undefined,
        platform: isBrowser ? 'browser' : 'node',
        sourcemap: false,
        dts: false,
        clean: false,
        tsconfig: isBrowser ? 'tsconfig.browser.json' : 'tsconfig.json',
        esbuildOptions(esOpts, _) {
            esOpts.target = target;
            esOpts.outfile = outfile;
            updateBuildOptions(esOpts);
        },
    });
}


async function buildDts() {
    let dtsFile = path.join('dist', 'index.d.ts');
    console.log(`Building dts to ${pc.blueBright(dtsFile)} ...`);

    dtsFile = path.join(__dirname, dtsFile);

    await build({
        entry: ['src/index.ts'],
        outDir: '',
        dts: true,
        clean: false,
        silent: true,
        tsconfig: 'tsconfig.dts.json',
        esbuildOptions(esOpts) {
            esOpts.outfile = dtsFile;
        }
    });

    await fse.move(dtsFile, path.join(__dirname, 'dist', 'types', 'index.d.ts'));
}

async function buildCli() {
    let outfile = path.join('dist', 'cli.js');
    console.log(`Building CLI to ${pc.blueBright(outfile)} ...`);
    outfile = path.join(__dirname, outfile);

    const bundle = false;
    const external = bundle ? ['path', 'fs', 'os'] : undefined;

    await build({
        entry: ['src/cli.ts'],
        outDir: '',
        format: 'cjs',
        silent: true,
        bundle: bundle,
        skipNodeModulesBundle: false,
        platform: 'node',
        external: external,
        minify: false,
        splitting: false,
        sourcemap: false,
        dts: false,
        clean: false,
        //tsconfig: 'tsconfig.build.json',
        esbuildOptions(esOpts) {
            esOpts.outfile = outfile;
            esOpts.platform = 'node';
            esOpts.external = external;
            updateBuildOptions(esOpts);
        },
    });
}

async function copyPackageJson() {
    // await execAsync('pnpm version patch', { cwd: __dirname });

    // const sourcePackageFile = path.join(__dirname, 'package.json');
    // const packageJson: Partial<IPackageJson> = await fse.readJson(sourcePackageFile, { encoding: 'utf-8' });

    // console.log('Updated local version to ' + pc.blueBright(packageJson.version));

    const newPackageJson: Partial<IPackageJson> = {
        name: packageJson.name,
        version: packageJson.version,
        author: packageJson.author,
        description: packageJson.description,
        engines: packageJson.engines,
        dependencies: packageJson.dependencies,
        peerDependencies: {
            'typescript': packageJson.peerDependencies?.typescript!,
        },
        types: 'types/index.d.ts',
        bin: {
            lhqcmd: 'cli.js'
        },
        main: "./index.js",
        browser: "./browser/index.js",
        module: "./esm/index.mjs",
    }

    let targetPackageFile = path.join('dist', 'package.json');
    console.log('Copying package.json to ' + pc.blueBright(targetPackageFile));
    targetPackageFile = path.join(__dirname, targetPackageFile);
    await fse.writeJson(targetPackageFile, newPackageJson, { encoding: 'utf-8', spaces: 2 });

    let targetHbsDir = path.join('dist', 'hbs');
    console.log('Copying hbs templates to ' + pc.blueBright(targetHbsDir));
    targetHbsDir = path.join(__dirname, targetHbsDir);

    await fse.copy(path.join(__dirname, 'hbs'), targetHbsDir);
}

async function genLhqSchema() {
    const schenameFileName = 'lhq-schema.json';
    console.log('Generating lhq model schema to ' + pc.blueBright(schenameFileName));

    const lhqSchemaFile = path.join(__dirname, 'dist', schenameFileName);
    const schemaJson = generateLhqSchema();
    await fse.writeFile(lhqSchemaFile, schemaJson);
    await fse.copy(lhqSchemaFile, path.join(__dirname, schenameFileName));
}