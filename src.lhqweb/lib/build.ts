import path from 'node:path';
import util from 'node:util';
import { exec, spawn } from 'node:child_process';

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
        await runMochaTests()

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

export async function runMochaTests(): Promise<void> {
    const cwd = __dirname;
    const bin = 'mocha/bin/mocha.js';
    const mocha = path.join(cwd, 'node_modules', bin);
    const testFile = './tests/index.spec.ts';

    const args = [mocha, '--delay', '-n tsx', '--enable-source-maps', '--fail-zero', '--colors', testFile];

    const { code, stdout, stderr } = await spawnAsync('node', args, { cwd, detached: false }, true);
    if (code !== 0) {
        throw new Error(`Some mocha tests failed (path '${cwd}')\n (code: ${code}) ${stdout || ''} ${stderr || ''}`);
    }

    const res = stdout.trim();
    if (res?.length > 0) {
        console.log(res);
    }
}

export type SpawnResult = { code: number, stdout: string, stderr: string, takes: number };

export type SpawnOptions = {
    cwd: string;
    detached: boolean;
    shell?: boolean;
    env?: NodeJS.ProcessEnv | undefined;
};


export function spawnAsync(command: string, args: string[], options: SpawnOptions, logToConsole: boolean = true, throwOnError: boolean = false): Promise<SpawnResult> {
    return new Promise((resolve, reject) => {
        let stderr = '';
        let stdout = '';
        const start = Date.now();

        if (logToConsole === undefined) {
            logToConsole = true;
        }

        if (throwOnError === undefined) {
            throwOnError = true;
        }

        /* eslint-disable */
        const ls = spawn(command, args, options || {});
        ls.stdout.on('data', (data) => {
            stdout += data.toString();

            if (logToConsole) {
                console.log(data.toString());
            }
        });

        ls.stderr.on('data', function (data) {
            stderr += data.toString();

            if (logToConsole && !throwOnError) {
                console.log(data.toString());
            }
        });
        /* eslint-enable */

        ls.on('exit', function (code) {
            const takes = Date.now() - start;

            if (code !== 0 && throwOnError) {
                return reject(stderr);
            }

            return resolve({ code: code ?? 0, stdout: stdout, stderr: stderr, takes: takes });
        });

        ls.on('error', err => {
            if (logToConsole) {
                console.error(err);
            }
            return reject(err);
        })
    });
}

// async function runExec(command: string, cwd: string) {
//     return new Promise<{ code: number; stdout: string; stderr: string }>((resolve, reject) => {
//         const child = exec(command, { cwd });

//         let stdout = '';
//         let stderr = '';

//         child.stdout?.on('data', (data) => {
//             stdout += data;
//         });

//         child.stderr?.on('data', (data) => {
//             stderr += data;
//         });

//         child.on('close', (code) => {
//             resolve({ code: code ?? 0, stdout, stderr });
//         });

//         child.on('error', (error) => {
//             reject(error);
//         });
//     });
// }