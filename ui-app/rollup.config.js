import serve from 'rollup-plugin-serve';
import livereload from 'rollup-plugin-livereload';
import { nodeResolve } from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';
import replace from '@rollup/plugin-replace';
import typescript from '@rollup/plugin-typescript';
import { terser  } from 'rollup-plugin-terser';

const DEV = 'development';
const PROD = 'production';
function parseNodeEnv(nodeEnv) {
  if (nodeEnv === PROD || nodeEnv === DEV) {
      return nodeEnv;
  }
  return DEV;
}
const nodeEnv = parseNodeEnv(process.env.NODE_ENV);
const plugins = [
  replace({
    'process.env.NODE_ENV': JSON.stringify(nodeEnv),
    preventAssignment: true
  }),
  nodeResolve({
     extensions: ['.js', '.tsx', 'ts']
  }),
  typescript(),
  commonjs({
    include: ['node_modules/**']
  })
];
switch(nodeEnv){
  case PROD:
    plugins.push(terser());
    break;
  case DEV:
  default:
    plugins.push(serve({
      open: true,
      verbose: true,
      contentBase: ['public', 'dist'],
      historyApiFallback: true,
      host: 'localhost',
      port: 3000
    }));
    plugins.push(livereload({ watch: 'dist' }));
    break;
  
}

export default {

  input: './src/index.tsx',
  plugins,
  output: {
    file: 'dist/bundle.js',
    format: 'iife',
    sourcemap: nodeEnv==DEV
  }
};
