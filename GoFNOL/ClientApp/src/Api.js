export class Api {
	async getUserData() {
		const response = await fetch('/api/user/data', {
			method: 'GET',
			credentials: 'same-origin'
		})
		return response.json()
	}

	async postCreateAssignmentRequest(request) {
		const response = await fetch('/api/fnol', {
			method: 'POST',
			credentials: 'same-origin',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(request)
		})
		return response.json()
	}

	postUserLogout() {
		fetch('/api/user/logout', {
			method: 'POST',
			credentials: 'same-origin'
		})
	}
}