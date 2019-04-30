import React, { Component } from 'react'
import { Route } from 'react-router'

import { AuthServiceInstance } from './authService'
import { Api } from './Api'

import { Home } from './components/Home'
import { AuthCallback } from './components/AuthCallback'

export default class App extends Component {

	render() {
		const api = new Api(AuthServiceInstance)

		return <React.Fragment>
			<header className="header shadowed">
				<h3>GoFNOL - {this.getEnvironmentName()}</h3>
				<button onClick={() => AuthServiceInstance.signOut()}>Logout</button>
			</header>
			<div className="content">
				<Route exact path='/' render={props => <Home {...props} api={api} authService={AuthServiceInstance} environment={this.getEnvironmentName()} />} />
				<Route path='/auth-callback' render={() => <AuthCallback />} authService={AuthServiceInstance} />
			</div>
		</React.Fragment>
	}

	getEnvironmentName = () => {
		const href = this.props.getWindowLocation().href
		if (href.indexOf('-acceptance') !== -1) {
			return Environment.Acceptance
		}

		if (href.indexOf('-int') !== -1) {
			return Environment.Int
		}

		if (href.indexOf('-demo') !== -1) {
			return Environment.Demo
		}

		return Environment.Local
	}
}

export const Environment = {
	Local: 'Local',
	Acceptance: 'Acceptance',
	Int: 'Int',
	Demo: 'Demo'
}