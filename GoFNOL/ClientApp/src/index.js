import React from 'react'
import ReactDOM from 'react-dom'
import { BrowserRouter } from 'react-router-dom'

import { createAuthService } from './authService'
import App from './App'

import 'bootstrap/dist/css/bootstrap.css'
import './index.css'

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href')
const rootElement = document.getElementById('root')

fetch('/api/config')
	.then(r => r.json())
	.then(createAuthService)
	.then(() => ReactDOM.render(
		<BrowserRouter basename={baseUrl}>
			<App getWindowLocation={() => window.location} />
		</BrowserRouter>, rootElement))
