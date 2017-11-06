import uglify from 'rollup-plugin-uglify';

import { isExternal, getGlobal } from './rollup-shared.config.js';
import { dependencies } from './rollup-dependencies.js'

export default {
    entry: 'dist/index.js',
    moduleName: 'cirrus.module.catch-em-all.ui',
    context: 'this',
    external: id => isExternal(id, dependencies),
    globals: id => getGlobal(id, dependencies),
    sourceMap: true,
    plugins: [
        uglify()
    ]
};