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
				<Route exact path='/' component={Home} />
			</div>
		</React.Fragment>
	}

	getEnvironmentName = () => {
		if (window.location.href.indexOf('-acceptance') !== -1) {
			return 'Acceptance'
		}

		if (window.location.href.indexOf('-int') !== -1) {
			return 'Int'
		}

		if (window.location.href.indexOf('-demo') !== -1) {
			return 'Demo'
		}

		return 'Local'
	}
}
