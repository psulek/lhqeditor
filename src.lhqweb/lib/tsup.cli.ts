import { defineConfig } from 'tsup';

export default defineConfig(() => {
    return {
        entry: ['build/cli.mts'],
        outDir: '',
        format: 'esm',
        bundle: true,
        skipNodeModulesBundle: true,
        platform: 'node',
        external: ['path', 'fs', 'os'],
        minify: false,
        splitting: false,
        sourcemap: false, // Equivalent to `devtool: false` in Webpack
        dts: false, // Disable .d.ts file generation
        clean: false, // Clean the output directory before building
        tsconfig: 'tsconfig.build.json', // Use the specified tsconfig.json
        esbuildOptions(options) {
            options.outfile = 'dist/cli.mjs';
            options.platform = 'node';
            options.external = ['path', 'fs', 'os'];
        },
    };
});