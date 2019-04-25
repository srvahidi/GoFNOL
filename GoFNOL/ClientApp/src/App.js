import React, { Component } from 'react'
import { Route } from 'react-router'
import { Home } from './components/Home'

export default class App extends Component {

	render() {
		return <React.Fragment>
			<header className="header shadowed">
				<h3>GoFNOL - {this.getEnvironmentName()}</h3>
				<form action="api/user/logout" method="post">
					<button type="submit">Logout</button>
				</form>
			</header>
			<div className="content">
				<Route exact path='/' render={props => <Home {...props} environment={this.getEnvironmentName()} />} />
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